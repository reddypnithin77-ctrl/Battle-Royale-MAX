export function isValidUnityClaims(payload, expectedProjectId) {
  return !!payload &&
    typeof payload.sub === "string" &&
    payload.sub.length > 0 &&
    payload.sub.length <= 128 &&
    typeof expectedProjectId === "string" &&
    expectedProjectId.length > 0 &&
    payload.project_id === expectedProjectId;
}

export function isValidFirebaseClaims(payload, expectedProjectId) {
  return !!payload &&
    typeof payload.uid === "string" &&
    payload.uid.length > 0 &&
    payload.uid.length <= 128 &&
    typeof expectedProjectId === "string" &&
    expectedProjectId.length > 0 &&
    payload.aud === expectedProjectId;
}

export function createIpRateLimiter({
  limit = 30,
  windowMs = 60_000,
  maxEntries = 10_000,
} = {}) {
  const windows = new Map();

  return function isRateLimited(ip, now = Date.now()) {
    const key = typeof ip === "string" && ip.length > 0 ? ip : "unknown";
    const current = windows.get(key);

    if (current && now >= current.start && now - current.start < windowMs) {
      current.count += 1;
      return current.count > limit;
    }

    if (!current && windows.size >= maxEntries) {
      for (const [storedIp, value] of windows) {
        if (now < value.start || now - value.start >= windowMs) {
          windows.delete(storedIp);
        }
      }
      if (windows.size >= maxEntries) return true;
    }

    windows.set(key, { start: now, count: 1 });
    return false;
  };
}
