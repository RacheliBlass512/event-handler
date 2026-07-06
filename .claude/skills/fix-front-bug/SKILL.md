---
name: fix-front-bug
description: Fix a frontend bug by first reproducing it in the real browser, finding the root cause (not the symptom), fixing it, then re-verifying in the browser. Use when the user reports a visual/behavioral bug in the Angular frontend.
---

# Fix Front Bug

Never fix a frontend bug from code reading alone. See it, understand it, fix it, see it fixed.

## Environment

- Frontend: Angular dev server on `http://localhost:4200`
- Backend: ASP.NET Core on `http://localhost:5027` (SignalR hub at `/hubs/events`)
- If either port isn't listening, ask the user to start it (don't start servers yourself unless asked).
- Browser: use the chrome-devtools MCP tools (`navigate_page`, `take_snapshot`, `take_screenshot`, `list_console_messages`, `list_network_requests`, `evaluate_script`).
- Login: seeded dev users, password `Passw0rd!` — `dispatcher` (Dispatcher), `tech1`/`tech2` (Technician). Expired JWT bounces you to `/login`; just log in again.

## Steps

1. **Reproduce.** Navigate to the app, log in with the role the bug affects, and walk the exact flow the user described until you see the bug. Capture evidence: screenshot + a11y snapshot + console errors/warnings + failed network requests. If you can't reproduce it, stop and report that — don't fix blind.

2. **Pin down the flow.** Note the minimal trigger sequence (which page, which action, which data state). If the bug needs specific data, note what (e.g. an event with a long name, an empty list).

3. **Root cause, not symptom.** Trace from the visible symptom back through the code: component template → component class → store/service → API/SignalR. Use `evaluate_script` to inspect live DOM/computed styles when it's a visual bug. Before editing, grep for every other place that shares the broken code path — the fix belongs where all affected paths route through, not in the one spot the report named.

4. **Fix.** Smallest change at the root cause. Keep it consistent with the surrounding code; if consistency requires a shared/global fix, do that rather than a local workaround (project rule).

5. **Re-verify in the browser.** Repeat the exact reproduction flow from step 1 and confirm the bug is gone: fresh screenshot, clean console, and check that nearby behavior didn't regress. A hard reload (`navigate_page` reload) picks up the rebuilt bundle — the dev server rebuilds on save.

6. **Report.** Show before/after evidence and explain the root cause in one or two sentences.
