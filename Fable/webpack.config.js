var path = require("path");
// const babelConf = {
//     presets: [ 
//       ["@babel/preset-env", {
//         "modules":false,
//         "corejs": 2,
//         "useBuiltIns": "usage"
//       }]
//   ]}
module.exports = [
    {
        mode: "development",
        devtool: "source-map",
        entry: path.join(__dirname, "./UI/UI.fsproj"),
        output: {
            path: path.join(__dirname, "./Fable"),
            filename: "fable-bundle.js",
        },
        devServer: {
            contentBase: "public",
            port: 8080,
        },
        module: {
            rules: [{
                test: /\.fs(x|proj)?$/,
                use: "fable-loader"
            }]
        }
    },
    {
        mode: "development",
        //devtool: "source-map",
        entry: path.join(__dirname, "./Components/Components.fsproj"),
        output: {
            path: path.join(__dirname, "../node/components"),
            filename: "fable-components.js",
            libraryTarget: "amd"
        },
        module: {
            rules: [{
                test: /\.fs(x|proj)?$/,
                use: "fable-loader"
            }]
        }
    }
]