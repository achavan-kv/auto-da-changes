"use strict";

var fs = require('fs');

var cartItemToAdd = fs.readFileSync(__dirname + '/data/CartItemToAdd.json', 'utf8');

exports.CartItemToAdd = JSON.parse(cartItemToAdd);

var orderToSave = fs.readFileSync(__dirname + '/data/OrderToSave.json', 'utf8');
exports.OrderToSave = JSON.parse(orderToSave);

var currentUser = fs.readFileSync(__dirname + '/data/CurrentUser.json', 'utf8');
exports.CurrentUser = JSON.parse(currentUser);

var users = {
    "99997": "Regional IT",
    "99998": "BBS Support",
    "99999": "System Administrator"
};
exports.Users = users;
