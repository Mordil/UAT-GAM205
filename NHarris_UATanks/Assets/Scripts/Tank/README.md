# AI Tanks

## Personalities

### Aggressive

* Does not flee from the player at all.
* Has `1.5` scaled X & Z for visual feedback.
* Does not stop chasing a player, even if they have left range.

### "French Tank"

* Flees twice as far as normal tanks.
* Flees twice as long as normal tanks.
* Has `.5` scaled X & Z for visual feedback

### Stationary

* Does not patrol.
* Does not chase players.
* Changes its color to a designer specified color

### Standard

* Chases players.
* Stops chasing players if they have left range for enough of a duration.
* Flees players.
* Normal red color.

### PhaseShift

* "Lerps" (not 100% linearly) between a min and max opacity at a designer set rate.
* Behaves as a Standard tank.

