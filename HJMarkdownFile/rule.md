# Work Rules

## ⚠️ MOST IMPORTANT RULE
**Always follow `@HJMarkdownFile/CLAUDE.md` when writing or modifying any code.**
Before touching any code, check these 4 principles:
1. **Think Before Coding** — State assumptions explicitly. If unclear, ask first.
2. **Simplicity First** — Implement only what was asked. No speculative features or abstractions.
3. **Surgical Changes** — Modify only lines directly tied to the request. Match existing style.
4. **Goal-Driven Execution** — For multi-step tasks, present a brief plan before implementing.

### 🚫 Scope Exclusions
- **Do NOT touch, modify, or reference**: `saleAlgorithm`, `CustomerManager`.
- These are strictly off-limits for LLM intervention.

---

## Response Style — Token Efficiency

- **No prose around code** — code speaks for itself
- **No end-of-response summaries** — user can read the diff
- **Bullets over paragraphs** — when explanation is needed
- **Ask before explaining** — if intent is unclear, one question beats a wrong long answer
- **Confirmations are one line** — "Done." or what changed, nothing more

---

## LLM Usage — 3-File Queue System

When one answer spawns multiple questions, use this system to avoid losing context.

### File Structure

```
HJMarkdownFile/
├── rule.md       ← This file (work rules)
├── focus.md      ← The ONE thing being solved in the current conversation
├── pending.md    ← Queue of questions to return to later
└── resolved.md   ← Resolved questions summary (knowledge base)
```

### Workflow

```
1. Start a session
   → Open a new conversation with: "@HJMarkdownFile/focus.md solve this"

2. New question arises mid-conversation
   → Don't break the flow — immediately append it to pending.md

3. Focus resolved
   → Move conclusion to resolved.md
   → Copy the top item from pending.md into focus.md
   → Start a new conversation

4. Returning to a broader topic
   → "@HJMarkdownFile/resolved.md @HJMarkdownFile/focus.md in this context..."
```

### Core Principles

- **One conversation = one focus** — Never try to solve multiple things at once
- **Context lives in files, not chat** — Long conversations cause earlier context to vanish
- **Write to pending immediately** — You will forget later
- **resolved = conclusions only** — No process, just the key decision and related files
