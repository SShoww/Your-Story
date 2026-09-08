# BePal

BePal is a single-player pet-care management simulation about learning the rules of abnormal pets through experimentation.

## Language

**Pet-Care Session**:
A focused interaction with one pet that ends when its Satisfaction is full.
_Avoid_: Round, encounter

**Satisfaction**:
A pet-specific measure of how close the current Pet-Care Session is to completion.
_Avoid_: Object bar, happiness bar

**Action Pattern**:
A pet-specific, fixed and repeating sequence that determines its Care QTE behaviour and may include attacks.
_Avoid_: Pet rule, behaviour pattern

**Care QTE**:
A wheel-based challenge in which the player selects the action required by the current Action Pattern to gain Satisfaction.
_Avoid_: Skill check, normal QTE

**Care Action**:
One of the four actions selectable in a Care QTE: Feed, Play, Pet, or Observe.
_Avoid_: Examine

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
The player's sole MVP damage resource; each Game Day starts with three Health, and each failed QTE removes one.
_Avoid_: Sanity

**Survival Log**:
A record of discovered pet behaviour that reveals a pet's complete Action Pattern after the player completes three Pet-Care Sessions with that pet.
_Avoid_: Journal, notebook

**Game Day**:
One unit of the run in which the player must complete at least one Pet-Care Session with the Active Pet before ending the day; additional sessions are optional, unless a Forced Retreat ends the day early.

**Forced Retreat**:
The early end of a Game Day caused by Health reaching zero; the player returns home and the following Game Day restores Health.

**Active Pet**:
The pet assigned to the current Game Day.
