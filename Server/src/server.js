import { createServer } from "node:http";
import { createRemoteJWKSet, jwtVerify } from "jose";
import pg from "pg";
import { createIpRateLimiter, isValidUnityClaims } from "./auth-security.js";

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

const isRateLimited = createIpRateLimiter({ limit: 30, windowMs: 60_000, maxEntries: 10_000 });

