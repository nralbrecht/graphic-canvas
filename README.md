# Graphic Canvas

This is a program in witch you can draw to scriptable canvas using a custom written interpreter.

## Usage

Running the `.exe` will open up the Interactive Shell where you can enter commands directly and live.

When you reference a source code file in the arguments this code will run and you can see the outcome in the window. Tip: Try draging a code file on the `.exe`

## Primitives

### Integer

Only Natural Numbers (n >= 0)

#### Example

- 0
- 123
- 424242

### Strings

A string of up to 256 characters.
Has to be surrounded by Quotation marks(").

#### Example

- "testing123"

### Color

A color given in its hexadecimal representation. See HTML color definitions for more detail.

#### Example

- #F0F
- #19BD8F
- #ff0
- #fa5e0d

## Variables

You can assign a value to an identifier.
Variables persist over the whole runtime and can be reassigned using the same assignment a second time.
After they are instantiated you can use them instead of a value.

### Example

- var foo = "bar"
- var bar = 123
- var test = #ff0

## Functions

To interact with the graphic canvas you can use a number of predefined functions.

- fill(color c)
- setPixel(int x, int y, color c)
- setRect(int x, int y, int width, int height, color c)
- writeText(int x, int y, int size, string text, color c)

- getPixel(int x, int y)
- getNoise(int x, int y)
- getWidth()
- getHeight()
- getRand(int min, int max)

## Example Code

A bit of example code that fills the whole screen with a single color. It changes the color of a single pixel and writes "testing123" in the top left corner using a randomly chosen font size.

```
var background = #111
var primary = #55f
var font_size = getRandom(10, 50)

fill(background)
setPixel(20, 20, primary)
write(1, 1, font_size, "testing123", primary)
```

## Key Bindings

- strg + s: saves the current canvas to an image in the current directory
- strg + r: reload the image
- alt + f4: closes the window

## TODO List

- Add math operations
- Add control flow elements (if, while, for, ...)
- Add Save function
- Make size adjustments