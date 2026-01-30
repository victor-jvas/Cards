# Technical Glossary

This glossary defines terminology used across architecture, rules, and conventions documentation.

---

## Ability
A capability associated with a card that produces effects when activated, triggered, or continuously applied.

---

## Check Timing
An automatic game window where mandatory rules and pending abilities are resolved without player interaction.

---

## Command
An object representing player intent. Commands never execute effects or mutate state directly.

---

## Continuous Effect
An effect that modifies game characteristics while active, applied via a layered system.

---

## Effect
A pure logic object describing a transformation of the game.

---

## Event
An immutable representation of something that is about to happen or has happened in the game.

---

## GameState
An immutable snapshot of the complete relevant game state at a given moment.

---

## Play Timing
A window where players have priority to declare commands.

---

## Replacement Effect
An effect that replaces one event with another before resolution.

---

## Rules Engine
The system responsible for validating commands, applying rules, and producing state transitions.

---

## Stack
A structure used to order effect resolution when applicable.

---

## Trigger
An ability that becomes pending when a specific event occurs.

---

## Zone
A logical location where a card may exist (deck, hand, battlefield, etc.).

---

## Determinism
The guarantee that identical inputs always produce identical results.

---

## Server Authority
A model in which only the server executes game logic and mutates state.

