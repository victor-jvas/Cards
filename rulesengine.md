# Rules Engine

This document formally describes the behavior of the game Rules Engine, with explicit focus on **Check Timing** and **Replacement Effects**, as defined by the Comprehensive Rules v1.5.

The goal is to eliminate implementation ambiguity and ensure the engine remains correct, deterministic, and extensible.

---

## 1. Rules Engine Responsibility

The Rules Engine is the **only system authorized** to:

- Validate player intent (Commands)
- Determine whether an action is legal
- Resolve effects
- Apply automatic rules
- Produce GameState transitions

No other system may:
- Mutate game state
- Resolve effects
- Decide timing

---

## 2. Game Timing Model

The game operates on two strictly separated timing concepts:

- **Play Timing** – when players may declare intent
- **Check Timing** – when the game automatically resolves rules

These timings must never overlap.

---

## 3. Check Timing

### 3.1 Definition

Check Timing is the automatic window in which the game:

1. Evaluates all mandatory rules
2. Applies automatic effects
3. Resolves pending abilities
4. Repeats until the game state stabilizes

Players never have priority during Check Timing.

---

### 3.2 Formal Process

During Check Timing, the Rules Engine executes the following loop:

1. Detect all applicable rule conditions
2. Generate all mandatory events
3. Apply all relevant Replacement Effects
4. Resolve resulting effects simultaneously
5. Register any newly generated events
6. Repeat until no new events are produced

This loop cannot be interrupted.

---

### 3.3 Rules Requiring Check Timing

Check Timing exists to support rules such as:

- Simultaneous resolutions
- Automatic eliminations
- Mandatory abilities
- State-based conditions

Without explicit Check Timing, the game will enter illegal states.

---

## 4. Replacement Effects

### 4.1 Definition

Replacement Effects modify **events**, not game state.

They are expressed as:

> "If X would happen, Y happens instead"

They never:
- Execute effects directly
- Generate events by themselves

---

### 4.2 Rules Defining Replacement Effects

Replacement Effects are defined primarily by:

- **Rule 10.11.1** – Definition of replacement effects
- **Rule 10.11.2** – Ordering when multiple replacements apply
- **Rule 10.11.3** – Single application per event
- **Rule 10.11.4** – Optional replacement effects

These rules mandate:
- Event interception before execution
- Immutable event representations

---

### 4.3 Replacement Pipeline

1. Original event is created
2. Engine identifies all applicable replacement effects
3. Affected object controller chooses ordering (when applicable)
4. Event is transformed
5. Resulting event proceeds to resolution

Each replacement effect may apply **only once per event**.

---

## 5. Relationship Between Check Timing and Replacement

Replacement Effects:
- Are evaluated **before** event resolution
- May generate new events that restart Check Timing

Therefore, replacement processing is a mandatory part of the Check Timing loop.

---

## 6. Common Implementation Errors

- Applying replacement effects directly to state
- Executing effects inside replacement logic
- Resolving replacements outside Check Timing
- Allowing the same replacement to apply multiple times

---

## 7. Architectural Guarantees

If this document is followed:

- The engine remains deterministic
- Complex rules do not become special cases
- Multiplayer state remains consistent
- New cards do not require core refactors

---

## 8. Conclusion

Check Timing and Replacement Effects are **foundational**.

Any implementation treating them as optional details is incorrect by definition.

