import test from "node:test";
import assert from "node:assert/strict";
import { createIpRateLimiter, isValidFirebaseClaims, isValidUnityClaims } from "../src/auth-security.js";

test("Firebase claims require a UID and the configured Firebase project", () => {
  assert.equal(isValidFirebaseClaims({ uid: "player-1", aud: "firebase-project" }, "firebase-project"), true);
  assert.equal(isValidFirebaseClaims({ uid: "player-1", aud: "other-project" }, "firebase-project"), false);
  assert.equal(isValidFirebaseClaims({ aud: "firebase-project" }, "firebase-project"), false);
  assert.equal(isValidFirebaseClaims({ uid: "", aud: "firebase-project" }, "firebase-project"), false);
  assert.equal(isValidFirebaseClaims({ uid: "x".repeat(129), aud: "firebase-project" }, "firebase-project"), false);
  assert.equal(isValidFirebaseClaims(null, "firebase-project"), false);
});

test("Legacy Unity claims helper remains strict", () => {
  assert.equal(isValidUnityClaims({ sub: "player-1", project_id: "project-1" }, "project-1"), true);
  assert.equal(isValidUnityClaims({ sub: "player-1", project_id: "other-project" }, "project-1"), false);
  assert.equal(isValidUnityClaims({ project_id: "project-1" }, "project-1"), false);
  assert.equal(isValidUnityClaims({ sub: "", project_id: "project-1" }, "project-1"), false);
  assert.equal(isValidUnityClaims({ sub: "x".repeat(129), project_id: "project-1" }, "project-1"), false);
  assert.equal(isValidUnityClaims(null, "project-1"), false);
});

test("IP limiter permits the configured number and blocks the next request", () => {
  const limited = createIpRateLimiter({ limit: 2, windowMs: 1000, maxEntries: 5 });
  assert.equal(limited("127.0.0.1", 100), false);
  assert.equal(limited("127.0.0.1", 101), false);
  assert.equal(limited("127.0.0.1", 102), true);
});

test("IP limiter resets after its time window", () => {
  const limited = createIpRateLimiter({ limit: 1, windowMs: 1000, maxEntries: 5 });
  assert.equal(limited("client", 100), false);
  assert.equal(limited("client", 101), true);
  assert.equal(limited("client", 1100), false);
});

test("IP limiter is bounded when many new addresses arrive", () => {
  const limited = createIpRateLimiter({ limit: 1, windowMs: 1000, maxEntries: 2 });
  assert.equal(limited("a", 100), false);
  assert.equal(limited("b", 100), false);
  assert.equal(limited("c", 100), true);
  assert.equal(limited("a", 1200), false);
  assert.equal(limited("c", 1200), false);
});
