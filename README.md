# Unity Ink Graph

Unity scripts for exporting a `.tgf` file for importing into the [yEd Graph Editor](https://www.yworks.com/products/yed) from a root `.ink` file.

Here is an example of 'The Intercept' as an exported graph in the yEd Graph Editor.

![image](https://user-images.githubusercontent.com/73082145/112088971-a23bab80-8bf5-11eb-9e62-5d7b9edfb165.png)

## Installation

1. Pull down this project.
2. Install the free [yEd Graph Editor](https://www.yworks.com/products/yed)
3. Associate yEd with `.tgf` files.
4. Add your ink scripts to this project or add the 'Unity Ink Graph' folder to your own project.
5. In the 'Project' window, click `+ Menu > Ink Graph Settings`
6. Assign your root `.ink` file to the `Root Ink Script` field in the new Ink Graph Settings file using the Inspector.

The Ink Graph Settings inspector should look something like this:

![image](https://user-images.githubusercontent.com/73082145/112089083-d6af6780-8bf5-11eb-9476-71a28fc5967c.png)

## To Use

### Exporting in Unity
1. Select the Ink Graph Settings file you created in the Project window
2. Set the settings you want to use to export in the Inspector window.
3. Click the 'Create Graph' button.

### Importing in yEd
1. If your graph didn't open automatically in the yEd Graph Editor...
    1. Open yEd Graph Editor
    2. `File > Open...` to the `.tgf` file you exported.<br/>(default is `<project>/Exports/<ink file name>.tgf`)
2. Select your desired import options. For complex stories, you probably want to ignore edge labels.

    ![image](https://user-images.githubusercontent.com/73082145/112089304-40c80c80-8bf6-11eb-9c96-9dab9049cc0b.png)

3. Select `Tools > Fit Node to Label` to resize nodes to the labels of the new graph.

    ![image](https://user-images.githubusercontent.com/73082145/112089256-27bf5b80-8bf6-11eb-85bf-e277acdce3ee.png)

4. Select `Layout > Hierarchical` or `Layout > Organic` to quickly group your graph.

    ![image](https://user-images.githubusercontent.com/73082145/112089322-49b8de00-8bf6-11eb-976c-495eab43e11f.png)

## License

[MIT License](https://github.com/fantabulous-tech/unity-ink-graph/blob/master/LICENSE)

Copyright (c) 2021 Fantabulous Tech

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

