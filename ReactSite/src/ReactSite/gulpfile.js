/// <binding BeforeBuild='appDevBuild' />
"use strict";

/*
    Notes:
    Browserify walks the require/import tree to bundle all the dependencies into an in memory file.  Combined with reactify it can transpile .jsx into .js.
    Vinyl-source-stream along with vinyl-buffer used to convert the browserify output to a buffered vinyl (virtual file format) object.
*/

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    eslint = require("gulp-eslint"),
    hash = require("gulp-rev"),
    replace = require("gulp-replace"),
    rename = require("gulp-rename"),
    source = require("vinyl-source-stream"),
    buffer = require("vinyl-buffer"),
    browserify = require("browserify");

var paths = {
    webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

paths.appPath = "JSX/app.jsx";
paths.appOutputName = "gulp_app.js";
paths.finalOutput = "wwwroot/build_gulp/";
paths.HomeOutput = "Pages/Home/";
paths.HomeOutputName = "Home.cshtml";
paths.HomeTemplatePath = "buildTemplates/gulp/";
paths.HomeTemplateName = "HomeTemplate.cshtml";

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:AppOutput", "clean:js", "clean:css"]);

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("min", ["min:js", "min:css"]);


//App build
gulp.task("setReactProductionMode", function () {
    process.env.NODE_ENV = 'production';
});

gulp.task("clean:AppOutput", function (cb) {   
    rimraf(paths.finalOutput, cb);
});

gulp.task("lint", ["clean"], function () {
    return gulp.src([paths.js, "JSX/**/*.jsx", "JSX/**/*.js"])
        .pipe(eslint())
        .pipe(eslint.format())
        .pipe(eslint.failAfterError());
});

//Transpiles jsx
//Bundles jsx, js, css
//Minifies bundle
//Adds hash to bundle name
//Creates manifest of old name to hash name
gulp.task("browserify", ["lint"], function () {
    return browserify(paths.appPath)
        .transform("babelify", { presets: ["es2015", "react"] })
        .transform(require("browserify-css"))
        .bundle()
        .pipe(source(paths.appOutputName))
        .pipe(buffer())
        .pipe(uglify())
        .pipe(hash())
        .pipe(gulp.dest(paths.finalOutput))
        .pipe(hash.manifest())
        .pipe(gulp.dest(paths.finalOutput));
});

//Replaces static bundle name in .cshtml home file with hashed bundle name
//Renames .cshtml file
gulp.task("replace", ["browserify"], function () {
    return gulp.src(paths.HomeTemplatePath + paths.HomeTemplateName)
        .pipe(replace("gulp_app.js", function (match) {
            var manifest = require("./" + paths.finalOutput + "rev-manifest.json");
            return manifest[match];
        }))
        .pipe(rename(paths.HomeOutputName))
        .pipe(gulp.dest(paths.HomeOutput, { overwrite: true }));
});

gulp.task("appDevBuild", ["replace"]);
gulp.task("appProdBuild", ["setReactProductionMode", "replace"])
