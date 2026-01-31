# AI Context

## 1. Glossary 

**System Context**
The highest‑priority instructions. They define who the AI is, what it must obey, safety rules, tool rules, and output rules. The AI can never break these.

**Custom Instructions (Platform / Copilot)**
Rules added by the platform or product. They shape behavior globally (style, limits, policies). They override agents and user prompts.

**Agent**
A specialized role or persona for the AI (e.g., coding assistant, DevOps agent). Agents add expertise and behavior but must obey system and custom instructions.

**Prompt (User Prompt)**
What the user asks for. Includes the task, inputs, and preferences. Powerful, but lowest authority compared to system and agents.

**Tool**
External capabilities the AI may use (files, terminal, browser, APIs). Tools are allowed or restricted by higher‑level instructions.

---

## 2. Main Components & Hierarchy (Combined)

AI context is built from multiple layers **and** those layers have a strict order of authority.

### Components (What exists in the context)

1. **System Prompt** – core identity, safety, rules
2. **Custom / Platform Instructions** – global enforcement rules
3. **Agent Instructions** – role, specialization, workflow
4. **Skills** – built-in or enabled competencies (e.g., coding, reasoning, translation, planning) that agents and prompts can invoke; skills expand *what* the AI can do but do not override rules.
5. **Developer Instructions** – app‑specific guidance (if present)
6. **User Prompt** – task, input data, constraints
7. **Conversation State** – recent chat history (short‑term memory)
8. **Retrieved Information (RAG)** – docs, files, search results
9. **Available Tools** – actions the AI may use
10. **Output Constraints** – required format or structure

### Hierarchy (Who overrides whom)

Highest priority always wins, and each layer *guides or constrains* the ones below it:

1. **System Context** (absolute authority) – sets the non‑negotiable rules of reality; all other layers must operate within it.
2. **Platform / Custom Instructions** – globally shape behavior and limits; they narrow how agents, skills, and users can act.

   * **Copilot Custom Instructions (Global)** – apply across all files and contexts; define overall style, safety, and behavioral rules.
   * **File‑Type Custom Instructions (Scoped)** – apply only to specific file types (e.g., `.ts`, `.md`, `.yaml`); refine behavior locally without overriding global Copilot instructions.
3. **Agent Instructions** – define the role and expertise; they decide *how and when skills are used* within higher‑level rules.
4. **Skills** – provide capabilities (what the AI *can* do); skills are invoked by agents or prompts but never override instructions.
5. **Developer Instructions** – adapt behavior to a specific application or workflow.
6. **User Prompt** – provides the actual task and inputs the AI should work on.
7. **Conversation History** – refines the task over time and provides short‑term continuity.
8. **Retrieved Data (RAG)** – supplies factual context and knowledge but never changes behavior rules.

If two layers conflict → the higher layer wins.

---

### Agent & Skills Interaction

**Key idea:** Agents do **not** automatically use all skills. They *selectively invoke* only the skills that fit their role and instructions, while ignoring or restricting others.

**How it works in practice:**

* The **agent role** decides which skills are relevant
* Higher-level instructions can **enable, limit, or forbid** certain skills
* Unapproved skills remain unused, even if they exist

**Example (enforcing specific skills):**

> *You are a documentation reviewer. Use **summarization** and **critique** skills only. Do **not** write new code.*

### One‑Line Memory Hook

**System defines reality → Platform enforces rules → Agent defines role → User asks → Data informs.**
