# CSP DOMAIN - ai architect

## variables

extrude: creates walls of building

curve: gives building curved walls via building outline

bevel: adds diagonal bevel to building corners

inset: adds a square inset to building outline

outset: adds a square outset to building outline

grow: inflates the outline of the building

shrink: deflates the outline of the building

## domians

the order in which the variables are fed into the builder class (to make the building from bottom up)

## constraints

-the building's outline cannot grow past its outline on the ground, but it can in the air

-any additive or subtractive outline operation (shrink, grow, outset, inset) is not allowed if it causes an outline to overlap itself.

-any operation forbidden by the stylistic guidelines is not allowed