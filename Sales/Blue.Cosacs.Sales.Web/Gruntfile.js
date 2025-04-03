module.exports = function (grunt) {
    require('load-grunt-tasks')(grunt);

    grunt.initConfig({
        watchify: {
            dev: {
                src: './js/index.js',
                dest: 'module.js',
                options: {
                    debug: true,
                    callback: function (b) {
                        var external = 'jquery, angular, bootstrap, ui-router, angular-bootstrap, Scope-onReady';
                        // configure the browserify instance here
                        //b.add();
                        //b.require();
                        //b.external('jquery');
                        // b.external('angular');
                        //b.ignore();
                        // b.transform();

                        // return it
                        return b;
                    }

                }
            },
            release: {
                src: './js/index.js',
                dest: 'module.js'
            }
        },
        watch: {
            app: {
                files: ['./js/**/*.js', './css/**/*.less'],
                tasks: ['default']
            }
        },
        uglify: {
            release: {
                files: {
                    'module.min.js': ['module.js']
                }
            }
        },
        less: {
            development: {
                options: {
                    relativeUrls: true,
                    paths: ["less"]
                },
                files: {"module.css": "css/main.less"}
            },
            release: {
                options: {
                    paths: ["less"],
                    relativeUrls: true,
                    compress: true
                },
                files: {"module.css": "css/main.less"}
            }
        },
        clean: ['module.css', 'module.js'],
        karma: {
            unit: {
                configFile: 'karma.conf.js'
            }
        }
    });

    // The default tasks to run when you type: grunt
    grunt.registerTask('dev', ['default','watch']);
    grunt.registerTask('default', ['clean', 'watchify:dev', 'less:development']);
    grunt.registerTask('release', ['clean', 'watchify:release', 'less:release', 'uglify']);
};