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
            path: path.join(__dirname, "../node/fable-include"),
            filename: "fable-bundle.js",
        },
        devServer: {
            contentBase: "../node/fable-include",
            port: 8080,
        },
        module: {
            rules: [{
                test: /\.fs(x|proj)?$/,
                use: "fable-loader"
            }]
        }
    },
    devServer: {
        contentBase: "public",
        port: 8081,
    },
    module: {
        rules: [{
            test: /\.fs(x|proj)?$/,
            use: "fable-loader"
        }]
    }
]