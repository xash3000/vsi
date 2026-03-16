# vsi

<ins>V</ins>ery <ins>S</ins>imple <ins>I</ins>nterpreter written in C#.

## Installation
You can download precompiled binaries from the releases page.
Alternatively, you can build it from source. You will need the [.NET SDK](https://dotnet.microsoft.com/download) installed.
After that, run this command to build the project:

```bash
$ dotnet build
```

## Quick Start

```ruby
# this is a comment

# variables
x := 1;
y := x + 2;

# printing
print x;
print y;

# if statements
if y > x then
    z := y;
# optional else
else
    z := x;
done

# while statemnt
while x < 5 do
    x := x + 1;
    print x;
done
```

save this to hello.vsi file and then

```bash
$ vsi hello.vsi 
```

output

```bash
1
3
2
3
4
5
```

see more examples at Examples/ folder

## LICENSE

**GNU GPLv3**

see LICENSE file for more information.
