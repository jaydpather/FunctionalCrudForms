var path = require("path");
module.exports = {
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
}