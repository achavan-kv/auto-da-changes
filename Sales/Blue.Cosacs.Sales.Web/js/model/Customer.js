'use strict';

var cls = Customer.prototype;

function Customer() {
    this.CustomerId = '';
    this.Alias = '';
    this.DOB = null;
    this.HomePhoneNumber = '';
    this.MobileNumber = '';
    this.Email = '';
    this.AddressLine2 = '';
    this.contacts = [];
    this.selected = false;
    this.Title = '';
    this.FirstName = '';
    this.LastName = '';
    this.AddressLine1 = '';
    this.TownOrCity = '';
    this.PostCode = '';
    this.isCardLinked = false;
    this.isSalesCustomer = false;

    function hasData(field) {
        return field && S(field).trim().s !== '';
    }
}

module.exports = Customer;