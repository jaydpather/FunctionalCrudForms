var path = require("path");
var babelOptions = {
    presets: [
      ["@babel/preset-env", {
        "targets": {
          "node": true,
        },
      }]
    ],
  };
module.exports = [
    {
        mode: "development",
        target: "node",
        node: {
            __dirname: false,
            __filename: false,
        },
        devtool: "source-map",
        entry: path.join(__dirname, "./UI/UI.fsproj"),
        output: {
            path: path.join(__dirname, "../node/fable-include"),
            filename: "fable-bundle.js",
            libraryTarget: "commonjs",
            library: "UI"
        },
        devServer: {
            contentBase: "public",
            port: 8081,
        },
        module: {
            rules: [{
                test: /\.fs(x|proj)?$/,
                use: {
                    loader: "fable-loader",
                    options: {
                        babel: babelOptions,
                        define: [] 
                    }
                }
            },
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: babelOptions
                }
            }]
        }
    },
]