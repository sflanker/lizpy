# Lizpy

Lizpy is a Clojure-like language for writing Vizzy programs. At present it is only possible to convert Lizpy to Vizzy not the other way around, but the intention is to be able to go back and forth with some naming constraints. The core of Lizpy can run as an external program and works with vanilla SimpleRockets2, however there are plans to support mods that extend Vizzy such as Vizzy++ and VizzyGL.

## Syntax

All of the syntactical aspects of Lizpy are borrowed from Clojure which is a kind of Lisp.

### Lists

Like any Lisp, the most fundamental aspect of the Lizpy syntax is lists. Lists begin with an open parentheses, contain items separated by whitespace, and end with a closing parentheses. Items within a list can optionally be delimited by commas. Lists can contain any syntactical element including other lists.

Here are some examples of lists:

```clojure
(one two three)
(4, 5, 6)
(= (+ 1 1) 2)
```

Note: Lists in Lizpy **do not** represent the data structures that can be stored in variables in Vizzy (those have [their own syntax](#arrays)). Lists are used to represent a variety of special forms including calling functions).

### Symbols

Symbols are groups of letters, numbers, and special characters. Symbols are used as variables, in special forms, for built in functions, and to define and call custom expressions and instructions. Symbols must not start with a number. Symbols cannot contain reserved characters which include `()[]{}\,;"`, and symbols used for variables or custom expression/instruction names have more restrictions (letters, numbers, and the underscore only).

#### Keywords

There are certain built-in functions that take symbols as arguments. In these cases the symbols are always prefixed with a colon (example: `:this-is-a-keyword`). This prevents them from being confused with variables. Keywords that are passed to built-in functions must be specified as constants, they cannot be returned as the result of a nested expression.

### Strings

Strings must be quoted with double quotes. Strings can be broken over multiple lines, and any whitespace following a newline will be reduced to a single space. Literal newlines can be included using an escape sequence.

#### Escape Sequences

| Character Sequence | Substitution |
| --- | --- |
| `\n` | newline |
| `\r` | carriage return | 
| `\t` | tab |
| `\\` | black slash |

### Numbers

Numbers are fairly self explanatory. Lizpy supports regular integers, decimals, and scientific notation.

### Boolean Values

Self explanatory: `true` and `false`. 

### Arrays

Array syntax is used to define argument lists in the custom expression and custom instruction special forms, as well as to declare literal sequences of values to be stored in Vizzy list variables. Arrays start with an open square bracket, contain items separated by whitespace, and end with a closing square bracket. Similar to lists, items within an array can optionally be delimited by commas. _Unlike a list_ arrays **cannot contain** arrays, lists, or other complex structures. Because of limitations of Vizzy, values in array literals will be converted to strings, and the strings in array literals **cannot contain commas**.

### Metadata

Metadata is a collection of data that can be attached to a symbol used to declare a custom expression/instruction or an event handler. Metadata does not affect the function of the program, however it can influence the way it is translated into Vizzy. Specifically metadata is used to control naming and positioning. Metadata syntax starts with a caret (`^`) followed by a map. A map starts with an open curly brace, followed by pairs of items (a key and a value) separated by spaces, and ending with a closing curly brace. Metadata always follows the symbol which it is attached to. **Note: Metadata is going to change in a future release to make it more similar to Clojure. Instead of attaching to the symbol before it it will attach to the form after it.**

#### Example
```clojure
(on-start ^{ :position [0 0] })
```

### Comments

Comments are completely ignored and will not be present in the generated Vizzy. Comments start with a single `;` and extend to the end of the line. _Note: Lizpy comments should not be confused with the built-in comment instruction which will be converted into the corresponding Vizzy instruction._

#### Example

```clojure
; This is a Lizpy comment and will be ignored
(comment "This is a Vizzy comment and will become an instruction")
```

## Special Forms

### Program

Like Vizzy, Lizpy is organized into Flight Programs. Each Flight Program takes the following form:

```clojure
(program "namespace"
  ; zero or more declaration or instruction forms
)
```

Each Lizpy program form should be in its own file. A program may optionally have a namespace specified as a string which will be used to prevent collision between variables instructions declared in different flight programs. If no namespace is specified the default, global, namespace will be used.

Like Vizzy, Lizpy programs may have unattached top level instructions. However, unlike Vizzy, Lizpy may not have unattached top level instructions or values.

### Import

One of the massive benefits of programming with Lizpy is the ability to modularize Flight Programs and share code. This is done by separating your program into multiple files which can be imported. The import form must be an item within the program form, and takes a file name or path as a string.

#### Example

```clojure
(program "my-namespace"
  (import "other-file.lizpy")

  ; declarations...
)
```

### Functions

Like any Lisp, functions are the most fundamental form in Lizpy. A function call is a list where the first item is a symbol identifying the function. In Lizpy both Vizzy Expressions and Instructions are treated like functions, although they can still only be used in the appropriate contexts. All of the other special forms listed here look very much like functions, but they have special meanings besides calling an expression or instruction.

### Do Block

In certain forms it is necessary to combine multiple instructions into a single form, the do block can take any number of instructions and turns them into a chain of instructions in Vizzy.

### Variable Declaration

The `declare` and `ldeclare` forms create global variables and list variables respectively. Variable names must start with a letter and contain only letters, numbers, and underscores.

#### Example

```clojure
(program
  (declare myVar)
  (ldeclare myListVar)
)
```


### Custom Instructions

Custom Instructions are declared with the `definst` form. This form takes a symbol used to identify the Custom Instruction, an array literal containing zero or more argument symbols, followed by zero or more function call forms. All functions called in  the root of a `definst` must be instructions themselves (just like in Vizzy, you cannot use an Expression where an Instruction is expected). Instruction symbols and argument symbols have the same character restrictions as variable symbols.

#### Example

```clojure
(definst myInstruction [arg1 ag2]
  (display "Hello World")
  (log (concat arg1 arg2)))
```

### Custom Expressions

Custom Expressions are declared with the `defexpr` form. This form takes a symbol used to identify the Custom Expression, an array literal containing zero or more argument symbols, followed by a single value or function call.

#### Example

```clojure
(defexpr myExpression [arg1 ag2]
  (concat arg1 arg2))
```

### Event Handlers

There are currently a number of forms for declaring different event handlers:

 * `on-start`
 * `on-part-collided`
 * `on-part-exploded`
 * `on-craft-docked`
 * `on-entered-soi`
 * `on-message-received`

With the exception of `on-message-received` all of these take the form of a list starting with one of the symbols above, followed by zero or more function calls. The `on-message-received` form also requires a string indicating the message.

Some events implicitly introduce arguments into the local scope. These arguments exactly match the arguments for the Vizzy counterparts.

**Note: event handlers may be undergoing a syntactic overhaul soon combining them into a single form, `on-event`, with a keyword to indicate the event.**

#### Example

```clojure
(program "namespace"
  (on-start
    (broadcast! "response" 42))
    
  (on-message-received "response"
    (display (format "The answer to the ultimate question is: {0}" data)))
)
```

## Builtin Instructions

### Program Flow Instructions

| Function&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Description |
| --- | --- |
| `(wait <number>)` | Wait `<number>` seconds. |
| `(wait-until <condition>)` | Wait until `<expression>` evaluates to `true`. |
| `(if <condition> <true-instruction> <false-instruction?>)` | If the expression evaluates to true, executes the specified instruction, otherwise executes the alternate instruction if specified. |
| `(cond <expr1> <inst1> <expr2> <inst2> ... <else-instruction?>)` | Similar to the if function, but it allows for multiple condition/true-instruction pairs. Analogous to an if/else if/else series. The else instruction is optional. |
| `(repeat <number> <instructions> ...)` | Repeat a set of instructions a fixed number of times |
| `(while <expression> <instructions> ...)` | Repeat a set of instructions so long as the condition evaluates to true. |
| `(for <start> <end> <interval> <instructions> ...)` | Repeat a set of instructions for numeric values from start to end (inclusive) changing by interval. |
| `(break)` | Exits the current loop or custom instruction. |
| `(comment <string>)` | An explanatory comment. This instruction is translated into the Vizzy comment instruction, which does have any effect on execution. |

### Display & Log Instructions

| Function | Description |
| --- | --- |
| `(display <string>)` | Displays the specified string. |
| `(log <string>)` | Adds the specified string to the Flight Programs log. |

### Variable Instructions

| Function&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Description |
| --- | --- |
| `(set! <symbol> <expression>)` | Sets the a global variable to the specified value. The variable must be explicitly declared. |
| `(change! <symbol> <interval>)` | Changes a global variable by adding the specified numeric value to the current value. |

###  List Instructions

List instructions modify the list stored in a list variable. _Note: because of the limitations of lists in Vizzy, list can only contain strings (although most value types can be implicitly converted to and from strings)._

| Function&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Description |
| --- | --- |
| `(list/add! <symbol> <expression>)` | Adds `<expression>` to the end of the list stored in the specified global variable. |
| `(list/insert! <symbol> <number> <expression>)` | Inserts `<expression>` into the list stored in the specified global variable at the specified position (index by 1). |
| `(list/remove-at! <symbol> <number>)` | Removes the item at the specified position (index by 1) from the list stored in the specified global variable. |
| `(list/set-at! <symbol> <expression>)` | Sets the specified global variable to the specified list expression (usually an Array in Lizpy, or the create function). |
| `(list/clear! <symbol>)` | Removes all items from the list stored in the specified global variable. |
| `(list/sort! <symbol>)` | Sorts the specified list in ascending order. |
| `(list/reverse! <symbol>)` | Reverses the order of the items in the specified list. |

### Broadcast Instructions

| Function&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Description |
| --- | --- |
| `(broadcast! <string> <expression>)` | Broadcast a message to event listeners in this flight program (including imported flight programs). |
| `(broadcast-to-craft! <string> <expression>)` | Broadcast a message to event listeners in all Flight Programs in the current craft. |

### Craft Instructions

| Function | Description |
| --- | --- |
| `(craft/activate-stage!)` | |
| [`(craft/set-input! <keyword> <number>)`](#set-input) |  |
| `(craft/set-target! <string>)` | |
| `(craft/set-heading! <number>)` | |
| `(craft/set-pitch! <number>)` | |
| [`(craft/set-autopilot-mode! <keyword>)`](#set-autopilot-mode) | |
| `(craft/set-activation-group! <number> <boolean>)` | |
| [`(craft/set-time-mode! <keyword>)`](#set-time-mode) | |
| `(craft/switch-craft! <number>)` | |
| [`(craft/set-part-property! <keyword> <number> <expression>)`](#set-part-property) | |
| [`(craft/set-camera-property! <keyword> <expression>)`](#set-camera-property) | |

#### Set Input

| Keyword | Description |
| --- | --- |
| `:Yaw` | Sets the craft's yaw input from -1 to 1 |
| `:Pitch` | Sets the craft's pitch input from -1 to 1 |
| `:Roll` | Sets the craft's roll input from -1 to 1 |
| `:Throttle` | Sets the craft's throttle input from 0 to 1 |
| `:Brake` | Sets the craft's brake input from 0 to 1 |
| `:Slider1` | Sets the craft's slider 1 input from -1 to 1 |
| `:Slider2` | Sets the craft's yaw slider 2 from -1 to 1 |
| `:TranslateForward` | Sets the craft's forward/back translation input from -1 to 1 |
| `:TranslateRight` | Sets the craft's right/left translation input from -1 to 1 |
| `:TranslateUp` | Sets the craft's up/down translation input from -1 to 1 |
| `:TranslationMode` | Enables or disables translation mode (Zero disables, non-zero enables). |

#### Set Autopilot Mode

| Keyword | Description |
| --- | --- |
| `:None` | Disables autopilot |
| `:Prograde` | Locks the orientation in the direction of the craft's velocity<sup>†</sup>. |
| `:Retrograde` | Locks the orientation in the opposite direction of the craft's velocity<sup>†</sup>. |
| `:Target` | Locks the orientation in the direction of current target. |
| `:BurnNode` | Locks the orientation in the necessary direction for the next burn node. |
| `:Current` | Locks the orientation to the current orientation. |

_† Velocity may be either surface velocity or orbital velocity depending on the user's current setting._

#### Set Time Mode

| Keyword | Description |
| --- | --- |
| `:Paused` | Pauses the game. |
| `:SlowMotion` | Sets the time rate to slow motion<sup>†</sup>. |
| `:Normal` | Sets the time rate to normal.|
| `:FastForward` | Sets the time rate to 2x speed. |
| `:TimeWarp1` | Sets the time rate to 2x speed. |
| `:TimeWarp2` | Sets the time rate to 10x speed. |
| `:TimeWarp3` | Sets the time rate to 25x speed. |
| `:TimeWarp4` | Sets the time rate to 100x speed. |
| `:TimeWarp5` | Sets the time rate to 500x speed. |
| `:TimeWarp6` | Sets the time rate to 2,500x speed. |
| `:TimeWarp7` | Sets the time rate to 10,000x speed. |
| `:TimeWarp8` | Sets the time rate to 50,000x speed. |
| `:TimeWarp9` | Sets the time rate to 250,000x speed. |
| `:TimeWarp10` | Sets the time rate to 1,000,000x speed. |

_† Slow motion rate will depend on the user's settings. Default is ¼ speed._

#### Set Part Property

Sets a property value for a part with the specified id. This function takes three arguments: a keyword, which must be a literal, not a nested expression or variable reference; a number that specified the id of the part; and an expression, the type and meaning of which depends on the property being set.

| Keyword | Description |
| --- | --- |
| `:Activated` | Set the activation state specified part. The expression for this property should be a boolean value indicating the activation state. |
| `:Name` | Sets the name for the part with the specified. The expression for this property should be a string. |
| `:Explode` | Causes the specified part to detach or explode. The expression should be a number indicating the power of the explosion. 0 just causes the part to fall without explosion. 1 is maximum explosion power. |

#### Set Camera Property

| Keyword | Description |
| --- | --- |
| `:RotationX` | Sets the camera's up/down rotation to the specified angle in radians. |
| `:RotationY` | Sets the camera's side to side rotation to specified angle in radians. |
| `:Tilt` | Sets the camera's tilt (roll) to the specified angle in radians. |
| `:Zoom` | Sets the camera's distance from the target in meters. |
| `:Mode` | Sets the camera mode by name. |
| `:ModeIndex` | Sets the camera mode by index. |

**Available Camera Modes**

 * "Orbit - Planet Aligned"
 * "Orbit - Space Aligned"
 * "Chase View"
 * "Fly By - Cinematic"
 * "Fly By - Stationary"

## Builtin Expressions

### Math

Math functions in lisp look a little different than normal algebra because the function symbol always comes first. For operations where order is important, the operation will be performed as if the first number is on the left hand side, and the second number is on the right hand side as the traditional notation (i.e. `(- 4 3)` in list is equivalent to `4 - 3` in standard notation and would return `1`).

| Function&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Description |
| --- | --- |
| `(+ <number> <number>)` | Addition. |
| `(- <number> <number>)` | Subtraction. |
| `(/ <number> <number>)` | Division. |
| `(* <number> <number>)` | Multiplication. |
| `(^ <number> <number>)` | Exponentiation. |
| `(% <number> <number>)` | Modulus (a.k.a. Remainder) |
| `(abs <number>)` | Absolute Value. |
| `(floor <number>)` | Rounds down to the nearest smaller integer value. |
| `(ceiling <number>)` | Rounds up to the nearest larger integer value. |
| `(sqrt <number>)` | Square Root. |
| `(log <number> <number?>)` | Returns the logarithm of the first number either using 10 as the base, or the second number as the base if specified. |
| `(ln <number>)` | Returns the natural logarithm (base [e](https://en.wikipedia.org/wiki/E_(mathematical_constant)) of the specified number. |
| `(exp <number>)` | Returns [e](https://en.wikipedia.org/wiki/E_(mathematical_constant)) raised to the specified power. |
| `(min <number> <number>)` | Returns the smaller of two numbers. |
| `(max <number> <number>)` | Returns the lager of two numbers. |
| `(sin <number>)` | The sine of an angle specified in radians. |
| `(cos <number>)` | The cosine of an angle specified in radians. |
| `(tan <number>)` | The tangent of an angle specified in radians. |
| `(asin <number>)` | The inverse sine of a value from -1 to 1. |
| `(acos <number>)` | The inverse cosine of a value from -1 to 1. |
| `(atan <number>)` | The inverse tangent of a value from -1 to 1. |
| `(atan2 <number> <number>)` | Returns the inverse tangent given the opposite and adjacent sides of a right triangle. `(atan2 x y)` is similar to `(atan (/ x y))` except that it returns the correct value when either or both of x and y are negative. |
| `(sinh <number>)` | The hyperbolic sine of an angle specified in radians. |
| `(cosh <number>)` | The hyperbolic cosine of an angle specified in radians. |
| `(tanh <number>)` | The hyperbolic tangent of an angle specified in radians. |
| `(asinh <number>)` | The inverse hyperbolic sine of a value from -1 to 1. |
| `(acosh <number>)` | The inverse hyperbolic cosine of a value from -1 to 1. |
| `(atanh <number>)` | The inverse hyperbolic tangent of a value from -1 to 1. |
| `(rand <number> <number>)` | Return a random number in between the specified bounds (inclusive). |

### Comparisons

These comparison operators only work with numbers.

| Function | Description |
| --- | --- |
| `(= <number> <number>)` | Equals. |
| `(!= <number> <number>)` | Not Equals. |
| `(< <number> <number>)` | Less than (returns `true` if the first number is less than the second). |
| `(<= <number> <number>)` | Less than or equals. |
| `(> <number> <number>)` | Greater than. |
| `(>= <number> <number>)` | Greater than or equals. |

### Conditionals and Boolean Logic

| Function | Description |
| --- | --- |
| `(if <condition> <true-expression> <false-expression>)` | A conditional that evaluates a condition expression and return the first subsequent expression if the condition is true, and the second subsequent expression if it is false. |
| `(cond <condition1> <expression1> <condition2> <expression2> ... <else-expression>)` | Similar to the `if` conditional except it can take any number of condition true-expression pairs. |
| `(and <condition> <condition>)` | Returns `true` if both conditions are `true`. |
| `(or <condition> <condition>)` | Returns `true` if either condition is `true`. |
| `(not <condition>)` | Negates the condition. |

### String Expressions

| Function&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Description |
| --- | --- |
| `(string= <string> <string>)` | Checks if two strings are equal. |
| `(string-contains <string> <sub-string>)` | Checks if the first string contains the second string. |
| `(string-length <string>)` | Returns the length of the string. |
| `(letter <number> <string>)` | Returns the nth letter of the string (index by 1). |
| `(substring <string> <start> <end?>)` | Gets a sequence of characters from the specified start up to the specified end (inclusive, index by 1). If end is not specified, the substring will be up to the end of the string. |
| `(concat <string1> <string2> ...)` | Concatenates two or more strings together. |
| `(format <format> <expression1> <expression2> ...)` | Generates a string using a [Microsoft .Net Composite Format String](https://docs.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting) and a series of expressions. |

### Vector Expressions

| Function&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Description |
| --- | --- |
| `(vector <number-x> <number-y> <number-z>)` | Creates a vector from three numbers. |
| `(vector/length <vector>)` | Returns the length (a.k.a. magnitude) of a vector. |
| `(vector/x <vector>)` | Returns the x axis component of a vector. |
| `(vector/y <vector>)` | Returns the y axis component of a vector. |
| `(vector/z <vector>)` | Returns the z axis component of a vector. |
| `(vector/normalize <vector>)` | Normalizes the vector to a length of 1 while preserving the direction. |
| `(vector/dot <vector> <vector>)` | Returns the [dot product](https://en.wikipedia.org/wiki/Dot_product) of two vectors. |
| `(vector/cross <vector> <vector>)` | Returns the [cross product](https://en.wikipedia.org/wiki/Cross_product) of two vectors. |
| `(vector/angle <vector> <vector>)` | Returns the angle **in degrees** between to vectors. |
| `(vector/dist <vector> <vector>)` | Returns the distance between to vectors. |
| `(vector/project <vector> <vector>)` | Returns the projection of the first vector onto the second vector (i.e. the component of the first vector that is in the same direction as the second vector). |
| `(vector/clamp <vector> <vector|number>)` | Returns a vector whose length is limited to that of the second vector or number (if the first vector's length is less than the second it will be returned unchanged. |
| `(vector/min <vector> <vector>)` | Returns the shorter of two vectors. |
| `(vector/max <vector> <vector>)` | Returns the longer of two vectors. |

### List Expressions

| Function | Description |
| --- | --- |
| `(list/create <string>)` | Creates a vector by splitting a string by commas. |
| `(list/get-item <list> <number>)` | Gets an item from a list by position (index by 1). |
| `(list/index-of <list> <string>)` | Returns the index of the specified string in a list (index by 1). |
| `(list/length <list>)` | Returns the length of a list. |

### Craft Expressions

| Function&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Description |
| --- | --- |
| [`(craft/info <keyword>)`](#current-craft-info) | Gets information about the current craft. |
| [`(craft/craft-info <keyword> <number>)`](#craft-info-by-id) | Gets information about other crafts by id. |
| `(craft/craft-id <string>)` | Gets the id of a craft by name |
| [`(craft/part-info <keyword> <number?>)`](#part-info) | Gets information about a part by id, or about parts on the craft in general. |
| `(craft/part-id <string>)` | Gets the id of a part by name |
| `(craft/activation-group <number>)` | Gets the state of the specified activation group. |

#### Current Craft Info

| Keyword | Description |
| --- | --- |
| `:Orbit.Apoapsis` | The altitude of the apoapsis above the planet's radius in meters. |
| `:Orbit.Periapsis` | The altitude of the periapsis above the planet's radius in meters. |
| `:Orbit.TimeToApoapsis` | The time remaining to the apoapsis in seconds. |
| `:Orbit.TimeToPeriapsis` | The time periapsis to the apoapsis in seconds. |
| `:Orbit.Eccentricity` | The eccentricity of the orbit. |
| `:Orbit.Inclination` | The inclination of the orbit in radians. |
| `:Orbit.Period` | The period of the orbit in seconds. |
| `:Atmosphere.AirDensity` | The air density of the atmosphere at the craft's current position in kg/m3. |
| `:Atmosphere.AirPressure` | The air pressure of the atmosphere at the craft's current position in Pa. |
| `:Atmosphere.SpeedOfSound` | The speed of sound at the craft's current position in m/s. |
| `:Atmosphere.Temperature` | The temperature at the craft's current position in K. |
| `:Altitude.AGL` | The craft's current altitude above ground level in meters. |
| `:Altitude.ASL` | The craft's current altitude above sea level in meters. |
| `:Performance.CurrentEngineThrust` | The current thrust generated by all active engines in N. |
| `:Performance.Mass` | The current mass of the craft in kg. |
| `:Performance.MaxActiveEngineThrust` | The thrust that could be achieved if all active engines were at 100% throttle, in N. |
| `:Performance.TWR` | The thrust to weight ratio of the craft. |
| `:Performance.CurrentIsp` | The current specific impulse of the craft in seconds. |
| `:Performance.StageDeltaV` | The delta-V remaining in the current stage. |
| `:Performance.BurnTime` | The remaining burn time in the current stage in seconds. |
| `:Fuel.Battery` | The percentage of battery remaining in the entire craft. |
| `:Fuel.FuelInStage` | The percentage of fuel remaining in the current stage. |
| `:Fuel.Mono` | The percentage of mono fuel remaining in the entire craft. |
| `:Fuel.AllStages` | The percentage of fuel remaining in the entire craft. |
| `:Vel.SurfaceVelocity` | The current velocity of the craft in m/s relative to the surface of the planet. |
| `:Vel.OrbitVelocity` | The current velocity of the craft in m/s in PCI coordinates. |
| `:Target.Velocity` | The current velocity of the target in m/s in PCI coordinates. |
| `:Vel.Gravity` | The current gravity in m/s in PCI coordinates. |
| `:Vel.Acceleration` | The current acceleration of the craft in m/s2. |
| `:Vel.AngularVelocity` | The current angular velocity of the craft in rad/s (pitch rate, yaw rate, roll rate). |
| `:Vel.LateralSurfaceVelocity` | The current velocity tangent to the planet surface in m/s. |
| `:Vel.VerticalSurfaceVelocity` | The current velocity toward the center of the planet in m/s. |
| `:Vel.MachNumber` | The current mach number. |
| `:Input.Roll` | The current roll input. |
| `:Input.Pitch` | The current pitch input. |
| `:Input.Yaw` | The current yaw input. |
| `:Input.Throttle` | The current throttle input. |
| `:Input.Brake` | The current brake input. |
| `:Input.Slider1` | The current slider 1 input. |
| `:Input.Slider2` | The current slider 2 input. |
| `:Input.TranslateForward` | The current translate forward input. |
| `:Input.TranslateRight` | The current translate right input. |
| `:Input.TranslateUp` | The current translate up input. |
| `:Nav.Position` | The current position of the craft's CoM from the center of the current planet in meters (PCI coordinates). |
| `:Target.Position` | The current position of the target from the center of its current planet in meters (PCI coordinates) |
| `:Nav.CraftHeading` | The current heading of the craft in degrees. |
| `:Nav.Pitch` | The current pitch of the craft in degrees. |
| `:Nav.BankAngle` | The current bank angle of the craft in degrees. |
| `:Nav.AngleOfAttack` | The current angle of attack of the craft in degrees. |
| `:Nav.SideSlip` | The current side slip of the craft in degrees. |
| `:Nav.North` | The unit vector representing north from the craft's current position, in PCI coordinates. |
| `:Nav.East` | The unit vector representing east from the craft's current position, in PCI coordinates. |
| `:Nav.CraftDirection` | The unit vector representing the roll axis of the craft in PCI coordinates. This is the direction the craft is currently pointing. |
| `:Nav.CraftRight` | The unit vector representing the pitch axis of the craft in PCI coordinates. |
| `:Nav.CraftUp` | The unit vector representing the yaw axis of the craft in PCI coordinates. |
| `:Name.Craft` | The name of the craft. |
| `:Orbit.Planet` | The name of the planet the craft is currently orbiting. |
| `:Target.Name` | The name of the current target. |
| `:Target.Planet` | The name of the target's current planet. |
| `:Misc.Grounded` | One indicates the craft is currently touching the ground. Zero indicates it is not. |
| `:Misc.SolarRadiation` | The current amount of solar radiation the craft is receiving. |
| `:Time.FrameDeltaTime` | The amount of time that has passed since the last frame of execution. |
| `:Time.TimeSinceLaunch` | The amount of time that has passed since this craft launched. |
| `:Time.TotalTime` | The amount of time that has passed since the sandbox was created. |

#### Craft Info By Id

| Keyword | Description |
| --- | --- |
| `:Altitude` | The craft's current altitude above sea level in meters. |
| `:Destroyed` | True if the craft is destroyed or does not exist. |
| `:Grounded` | True if the craft is currently in contact with the planet. |
| `:Mass` | The mass of the craft in kilograms. |
| `:PartCount` | The number of parts in the craft. |
| `:Planet` | The name of the planet the craft is currently orbiting. |
| `:Position` | The current position of the craft from the center of its parent planet in meters (PCI coordinates). |
| `:Velocity` | The current velocity of the craft in m/s in PCI coordinates. |

#### Part Info

| Keyword | Description |
| --- | --- |
| `:Mass` | The mass of the part in kg. |
| `:Activated` | True if the part with the specified ID is activated. |
| `:PartType` | The type of the part with the specified ID. |
| `:Position` | The current position of the part from the center of the current planet in meters (PCI coordinates). |
| `:Temperature` | The temperature of the part with the specified ID in Kelvin. |
| `:UnderWater` | The percentage of the part that is underwater from 0 to 1. Zero meaning not under water at all, and one meaning fully submerged. |
| `:MinID` | No id required. Gets the smallest ID of all parts in the craft. |
| `:MaxID` | No id required. Gets the largest ID of all parts in the craft. |
| `:ThisID` | No id required. The ID of the part running this flight program. |

### Planet Expressions

| Function | Description |
| --- | --- |
| `(planet/info <keyword> <string>)` | Gets information about a planet by name. |

**Available Properties**

| Keyword | Description |
| --- | --- |
| `:Mass` | The mass of the planet in kilograms. |
| `:Radius` | The radius of the planet in meters. |
| `:AtmosphereHeight` | The height of the atmosphere in meters. |
| `:SolarPosition` | The position of the planet relative to the sun. |
| `:ChildPlanets` | The list of names of planet's children planets. |
| `:Crafts` | The list of names of crafts inside the SOI of the planet. |
| `:CraftIDs` | The list of IDs of crafts inside the SOI of the planet. |
| `:Parent` | The name of the planet's parent. |


### Coordinate Conversion

| Function&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Description |
| --- | --- |
| `(pci->lat-lon-agl <vector>)` | Converts the PCI vector to a (latitude, longitude, AGL) vector. |
| `(lat-lon-agl->pci <vector>)` | Converts the (latitude, longitude, AGL) vector to a PCI vector. |
| `(local->pci <number> <vector>)` | Converts the vector from local to PCI coordinates using the frame of reference of the specified part (by id). If the part id is 0 the craft's frame of reference is used. |
| `(pci->local <number> <vector>)` | Converts the vector from PCI to local coordinates using the frame of reference of the specified part (by id). If the part id is 0 the craft's frame of reference is used. |

## Namespaces

Expressions, instructions, and global variables can be declared either in the global namespace or as specific namespace. When an expression, instruction, or variable is declared in a specific namespace it must be referenced with it's namespace using the syntax `namespace/identifier`. There is not currently any support for foregoing namespace qualification or for aliasing imported declarations, although this may be added in the future. When generating Vizzy namespaced identifiers are named using the format `namespace__identifier`.

## Building and Running the Lizpy Compiler

I'm not yet providing binary releases, so for the time being you'll need to build and run the `clizpy` command line yourself. I'm compiling on Linux using Mono and JetBrains Rider. Soon I will list cross platform build instructions and provide binaries (either cross platform or at least binaries for Linux/Mac/Mono and Windows. Regardless it will be a prerequisite to be able to run .Net Framework 4.7.1 executables.

Once compiled you can copy `clizpy.exe` and it's dependencies to your desired installation directory, add that to your `PATH` variable, and then use either:

```batch
REM On Windows
clizpy.exe SourceFile.lizpy
```

or

```bash
# On Linux/Mac
mono clizpy.exe SourceFile.lizpy
```

The Lizpy compiler command line writes it's output to stdout, so you can redirect the output to a file to save the Flight Program: `mono clizpy.exe SourceFile.lizpy > MyProgram.xml`.

## Roadmap

This is only vaguely in priority order and some of these are just crazy ideas.

1. Rearrange metadata to make it work like Clojure
1. `displayf` and `logf` variants that compose `display`/`log` and `format`
1. Case statements.
1. Let block (for the declaration of helper variables in expressions).
    * A let block would be transformed into a custom expression with arguments for each local variable declaration.
1. Higher order functions, anonymous functions, partial application, and function composition.
    * Declare a function that takes a function as an argument (higher order function).
    * Where ever that function is used, an anonymous function or existing expression name must be specified for that argument.
    * For each usage of the higher order function, a unique custom expression is created where the named or anonymous function is inlined into the expression definition.
    * This will make `reduce` possible, and will also be useful for things like `map` and `filter` when we get list construction support.
1. Make the `for` loop construct take an anonymous function instead of a list of instructions so that the symbol for the counter can be user specified, instead of always being `i`
1. Automatic position generation (for each top level declaration, if it has no explicit position, generate a position so that it is below the declaration before it).
1. Vizzy -> Lizpy decompilation
1. Better compiler errors
    * Currently compilation fails when it hits the first error, it would be better to return multiple errors (of course this is in keeping with Clojure which has terrible compiler error output).
1. More built-in helper functions
    1. Vector with-axis (i.e. `(vector/with-component vec :x 1)` is the same as `(vector 1 (vector/y vec) (vector/z vec))`)
    1. Vector reject (returns the component of one vector that is perpendicular to another).
    1. Math & physics constants: PI, e, c, G
1. Support for VizzyGL and Vizzy++ Extensions
1. Strong typing/type checking
    * Help people avoid mistakes like using a vector where a number is required or vice versa.  
    * Static typing? That wouldn't be very Clojure-esque. 
    * Type hints?
    * Type inference?
    * It would be nice to provide multiple implementations depending on the type, like the equals operator (which needs different implementations for numbers vs. strings). However runtime type checking isn't really an option since it would hurt performance.
1. Macros (i.e. Lizpy that runs at compile time to generate more Lizpy).
1. Import namespace aliasing
1. Support for display message styling (TextMeshPro)
1. Support for associative arrays (a.k.a maps or dictionaries)
    * Stored as sorted lists of key/value pairs (every other item is a key)
1. Asynchronous programming model
    * Option 1. Javascript-esque Promises/Awaits
        * Creating a Promise is basically broadcasting a message that performs some computation and stores a result in a variable.
        * Awaiting a Promise is waiting for the result to be ready
    * Option 2. Implement something like Clojure's core.async
1. Synchronization primitives: semaphore, lock, monitor?

### SimpleRockets2 Integration

Making it possible to edit FlightPrograms as Lizpy inside the game should make Lizpy much easier to use. This will be a separate project that depends on this one. The basic functional requirements are:

* An "Edit Lizpy" menu option in the Flight Program editor
    * This opens a full screen text editor view with tabs to open multiple flight programs.
* A separate "Lizpy" directory inside the "FlightPrograms" directory.
* Synchronization between the Vizzy and Lizpy for each Flight Program
    * If a flight program has no associated Lizpy program associated with it, generate one.
    * When a lizpy program is saved to the craft, vizzy gets generated, but a lizpy file should also be generated in `FlightPrograms/Lizpy/CraftPrograms/CraftId_PartId.lizpy`
    * If a lizpy file already exists for the craft/part containing the flight program being edited, check whether changes have been made to the Vizzy (not sure how to do this). If changes have been made, prompt the user whether they want to redo the Vizzy->Lizpy conversion or open the existing file.
* The Lizpy editor should allow other saved flight programs to be opened in other tabs. Lizpy flight programs can be saved as named programs, or just saved to the craft, just like Vizzy.
* A Lizpy program that has errors can be saved as a named program, but it cannot be saved to a craft.
* All imports should default to being relative to the `FlightPrograms/Lizpy` folder.

Vizzy->Lizpy conversion could be quite tricky. Naively, a hand created Vizzy program should convert fairly easily. However, it will be much more difficult to make it so that a program that was written in Lizpy and converted to Vizzy converts back to Lizpy looking much the same. Obviously Lizpy comments (`;; lines`) would be lost. Another big gotcha is namespaces and imported programs. I think the easiest thing would be to support multiple program declarations in a single Lizpy file and basically smash the imported declarations all into the same file. If the user wants to go back to relying on imports they can delete the other `(program ...)` declarations and re-add the `(imports ...)`. I think pushing people towards a Lizpy only experience is probably better than trying to make the Vizzy<->Lizpy conversions flawless.

### Blocked Until Vizzy Support

1. List constructing expressions (i.e. `cons`) and by extension expressions like `map`/`filter` please upvote [this suggestion](https://www.simplerockets.com/Feedback/View/6LZ4ph/Vizzy-expression-for-building-lists-and-taking-spans-from-lists).
