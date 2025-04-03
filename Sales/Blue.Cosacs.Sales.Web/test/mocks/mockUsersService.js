'use strict';

function mockUsersService(sandbox){
    return ({
        getCurrentUser: sandbox.stub(),
        logOutUser: sandbox.stub(),
        setUser: sandbox.stub(),
        hasPermission:sandbox.stub()

    });
}

module.exports = mockUsersService;