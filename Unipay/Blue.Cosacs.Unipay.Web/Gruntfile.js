module.exports = function (grunt) {
    require('load-grunt-tasks')(grunt);

    grunt.initConfig({
        watchify: {
            dev: {
                src: './index.js',
                dest: 'module.js',
                options: {
                    debug: true,
                    callback: function (b) {
                        return b;
                    }

                }
            },
            release: {
                src: './index.js',
                dest: 'module.js'
            }
        },
        watch: {
            app: {
				files: ['./views/*.html', './views/*.xml'],
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
        clean: ['module.js']
    });

    // The default tasks to run when you type: grunt
    grunt.registerTask('dev', ['default','watch']);
    grunt.registerTask('default', ['clean', 'watchify:dev']);
    grunt.registerTask('release', ['clean', 'watchify:release', 'uglify']);
};