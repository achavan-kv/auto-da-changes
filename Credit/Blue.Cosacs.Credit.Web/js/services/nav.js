'use strict';
var navService = function () {
    var local = {};
    return {
        init: function (data) {
            local = data; // Set everything.
        },
        setApp1Name: function (name) {
            local.applicant1Name = name;
        },
        setApp2Name: function (name) {
            local.applicant2Name = name;
        },
        setHasApplicant2: function (hasApplicant2) {
            local.hasApplicant2 = hasApplicant2;
        },
        screen: function () {
            return local.screen;
        },
        getApp1Name: function () {
            if (local.applicant1Name == null || local.applicant1Name.trim().length == 0) {
                return "Applicant 1"
            }
            return local.applicant1Name;
        },
        getApp2Name: function () {
            if (local.applicant2Name == null || local.applicant2Name.trim().length == 0) {
                return "Applicant 2"
            }
            return local.applicant2Name;
        },
        hasApplicant2: function () {
            return local.hasApplicant2;
        },
        setApplicationType: function (applicationType) {
            local.applicationType = applicationType;
        },
        setSectionManager: function (sectionManager) {
            local.sectionManager = sectionManager;
        },
        getSanction1App2Sections: function () {
            if (local.applicationType) {
                return local.sanction1AppSections[local.applicationType];
            }
        },
        isSectionCompleted: function (val) {
            if (!local.sectionManager) {
                return false;
            }
            return local.sectionManager.isSectionCompleted(val);
        },
        canScore: function () {
            if (!local.sectionManager) {
                return false;
            }
            return local.sectionManager.canScore();
        },
        canDC: function () {
            if (!local.sectionManager) {
                return false;
            }
            return local.sectionManager.canDC();
        },
        save: function () {
            return local.save;
        }
    }
};
module.exports = navService;