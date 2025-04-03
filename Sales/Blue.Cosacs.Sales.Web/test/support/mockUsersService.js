'use strict';

function mockUsersService() {
    return {
        getCurrentUser: function () {
            return {
                "BranchNumber": 900,
                "BranchName": "BRIDGETOWN 900",
                "name": "System Administrator",
                "permissions": []
            };
        },
        logOutUser: function () {
        },
        setUser: function () {
        },
        hasPermission: function () {
        }

    };
}

module.exports = mockUsersService;