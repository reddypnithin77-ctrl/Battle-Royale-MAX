import { createServer } from "node:http";
import { createRemoteJWKSet, jwtVerify } from "jose";
import pg from "pg";

const { Pool } = pg;
const PORT = Number(process.env.PORT || 8080);
const EXPECTED_PROJECT_ID = process.env.UNITY_PROJECT_ID;
const DATABASE_URL = process.env.DATABASE_URL;
const ISSUER = "https://player-auth.services.api.unity.com";
const JWKS = createRemoteJWKSet(
  new URL("https://player-auth.services.api.unity.com/.well-known/jwks.json")
);
const pool = new Pool({
  connectionString: DATABASE_URL,
  ssl: process.env.PGSSLMODE === "disable" ? false : { rejectUnauthorized: true },
  max: 10,
  connectionTimeoutMillis: 5000,
  idleTimeoutMillis: 30000,
});

if (!EXPECTED_PROJECT_ID) throw new Error("Set UNITY_PROJECT_ID.");
if (!DATABASE_URL) throw new Error("Set DATABASE_URL to a private PostgreSQL database.");

const ipWindows = new Map();
const RATE_LIMIT = 30;
const WINDOW_MS = 60_000;

function isRateLimited(ip, now = Date.now()) {
  const current = ipWindows.get(ip);
  if (!current || now - current.start >= WINDOW_MS) {
    ipWindows.set(ip, { start: now, count: 1 });
    return false;
  }
  current.count += 1;
  if (ipWindows.size > 10000) {
    for (const [key, value] of ipWindows) {
      if (now - value.start >= WINDOW_MS) ipWindows.delete(key);
    }
  }
  return current.count > RATE_LIMIT;
}

function send(response, status, body) {
  response.writeHead(status, {
    "Content-Type": "application/json; charset=utf-8",
    "Cache-Control": "no-store",
    "X-Content-Type-Options": "nosniff",
    "Referrer-Policy": "no-referrer",
  });
  response.end(JSON.stringify(body));
}

async function initializeDatabase() {
  await pool.query(`
    CREATE TABLE IF NOT EXISTS player_profiles (
      player_id TEXT PRIMARY KEY,
      created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
      last_seen_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
    )
  `);
}

const server = createServer(async (request, response) => {
  if (request.method === "GET" && request.url === "/health") {
    try {
      await pool.query("SELECT 1");
      send(response, 200, { ok: true, database: "connected" });
    } catch {
      send(response, 503, { ok: false, database: "unavailable" });
    }
    return;
  }

  if (request.method !== "POST" || request.url !== "/auth/verify") {
    send(response, 404, { error: "not_found" });
    return;
  }

  // Use the socket address, not caller-controlled X-Forwarded-For.
  const clientIp = request.socket.remoteAddress || "unknown";
  if (isRateLimited(clientIp)) {
    send(response, 429, { error: "rate_limited" });
    return;
  }

  const authorization = request.headers.authorization || "";
  const match = /^Bearer ([A-Za-z0-9._~-]+)$/.exec(authorization);
  if (!match) {
    send(response, 401, { error: "missing_bearer_token" });
    return;
  }

  try {
    const { payload } = await jwtVerify(match[1], JWKS, {
      issuer: ISSUER,
      algorithms: ["RS256"],
    });

    if (typeof payload.sub !== "string" || payload.sub.length === 0 ||
        payload.project_id !== EXPECTED_PROJECT_ID) {
      send(response, 401, { error: "invalid_identity_claims" });
      return;
    }

    // Identity is taken only from the verified token; the client cannot choose a profile ID.
    const result = await pool.query(
      `INSERT INTO player_profiles (player_id)
       VALUES ($1)
       ON CONFLICT (player_id)
       DO UPDATE SET last_seen_at = NOW()
       RETURNING player_id, created_at, last_seen_at`,
      [payload.sub]
    );

    send(response, 200, {
      authenticated: true,
      player: {
        playerId: result.rows[0].player_id,
        createdAt: result.rows[0].created_at,
        lastSeenAt: result.rows[0].last_seen_at,
      },
    });
  } catch (error) {
    if (error?.code && String(error.code).startsWith("23")) {
      send(response, 503, { error: "profile_storage_unavailable" });
      return;
    }
    // Do not reveal token-validation details to unauthenticated callers.
    send(response, 401, { error: "invalid_or_expired_token" });
  }
});

await initializeDatabase();
server.listen(PORT, "0.0.0.0", () => {
  process.stdout.write(`Auth service listening on port ${PORT}\\n`);
});

async function shutdown() {
  server.close();
  await pool.end();
  process.exit(0);
}
process.on("SIGTERM", shutdown);
process.on("SIGINT", shutdown);  let payload;
  try {
    const verified = await jwtVerify(match[1], JWKS, {
      issuer: ISSUER,
      algorithms: ["RS256"],
    });
    payload = verified.payload;
  } catch {
    send(response, 401, { error: "invalid_or_expired_token" });
    return;
  }

  if (typeof payload.sub !== "string" || payload.sub.length === 0 ||
      payload.project_id !== EXPECTED_PROJECT_ID) {
    send(response, 401, { error: "invalid_identity_claims" });
    return;
  }

  try {
    // Identity is taken only from the verified token; the client cannot choose a profile ID.
    const result = await pool.query(
      `INSERT INTO player_profiles (player_id)
       VALUES ($1)
       ON CONFLICT (player_id)
       DO UPDATE SET last_seen_at = NOW()
       RETURNING player_id, created_at, last_seen_at`,
      [payload.sub]
    );

    send(response, 200, {
      authenticated: true,
      player: {
        playerId: result.rows[0].player_id,
        createdAt: result.rows[0].created_at,
        lastSeenAt: result.rows[0].last_seen_at,
      },
    });
  } catch {
    send(response, 503, { error: "profile_storage_unavailable" });
  }
