# BePal

BePal is a single-player pet-care management simulation about learning the rules of abnormal pets through experimentation within a cozy yet dangerous shelter.

## Core Philosophy

**Cozy yet Dangerous**:
The contrast between a warm, comfortable shelter/home and uncanny, abnormal creatures with lethal potential.
_Avoid_: Pure horror, cute pet sim

**Consequential Interaction**:
Every choice in caring for a pet yields distinct, observable consequences—trial and error is required to deduce safe care routines.

## Language & Domain Terms

**Pet-Care Session**:
A focused interaction with one pet that ends when its Satisfaction is full.
_Avoid_: Round, encounter

**Satisfaction**:
A pet-specific measure of how close the current Pet-Care Session is to completion (gained via effective Care Actions).
_Avoid_: Object bar, happiness bar

**Action Pattern**:
A pet-specific, fixed and repeating sequence that determines its Care QTE behaviour and may include attacks.
_Avoid_: Pet rule, behaviour pattern

**Care QTE**:
A wheel-based challenge in which the player selects the action required by the current Action Pattern to gain Satisfaction.
_Avoid_: Skill check, normal QTE

**Care Action**:
One of the four actions selectable in a Care QTE:
- **Feed (Appetite)**: Providing sustenance and monitoring dietary reaction.
- **Play (Recreation)**: Interacting with toys or activities to satisfy stimulation needs.
- **Pet (Intimacy)**: Approaching and physically soothing the pet to build trust.
- **Observe (Observation)**: Watching without touch to study behavior and spot abnormal shifts.
_Avoid_: Examine

**Pet Favor**:
The tier of satisfaction gained from a Care Action: Very Effective (+2), Effective (+1), Neutral (+0), or Rejection/Attack (0 gain with damage or triggering Dodge QTE).

**Hazard Level**:
The danger classification of an abnormal pet (Level 1 to 3), determining damage potency and aggression.

**Harm Type**:
The nature of damage inflicted during care failures: Physical (bodily damage) or Mental (sanity strain).

**Dodge QTE**:
A reactive challenge triggered by a pet attack in which the player confirms while the Wheel Marker is inside a wide Dodge Zone; completing it avoids the attack's damage.
_Avoid_: Attack QTE

**Dodge Zone**:
The marked safe area of a Dodge QTE wheel.

**Wheel Marker**:
The moving indicator on a Care QTE wheel that determines the currently selected action.
_Avoid_: Arrow, needle

**Teleporting Marker**:
An Action Pattern modifier in which the Wheel Marker jumps once to a random wheel position during each Care QTE before continuing to move.

**QTE Confirmation**:
The Spacebar input used to submit a Care QTE or Dodge QTE attempt.
_Avoid_: Click to confirm

**Health (HP)**:
The player's damage resource; each Game Day provides Health, and failed Care QTEs or missed Dodge QTEs deplete it.

**Energy**:
The daily allowance of care interactions available to the player per Game Day.

**Survival Log**:
A record of discovered pet behaviour that documents pet preferences, harm types, and action patterns after repeated Pet-Care Sessions.
_Avoid_: Journal, notebook

**End-of-Day Summary**:
The daily debriefing screen that records discoveries, tracks retreat counts, and transitions to the next day.

**Game Day**:
One unit of the run in which the player must complete at least one Pet-Care Session with the Active Pet before ending the day; additional sessions are optional, unless a Forced Retreat ends the day early.

**Forced Retreat**:
The early end of a Game Day caused by Health reaching zero; the player returns home/recovers, and the following Game Day restores Health.

**Active Pet**:
The pet assigned to the current Game Day.
