(function() {
  var StateMachine;
  StateMachine = (function() {
    function StateMachine() {}
    StateMachine.prototype.currentstate = '';
    StateMachine.prototype.constuctor = function(baseUrl) {
      var currentstate;
      this.baseUrl = baseUrl;
      return currentstate = changestate(directing, '');
    };
    StateMachine.prototype.docRoutes = {
      PL: '/courts.net.ws/home/test/%',
      DL: '/courts.net.ws/home/test/%',
      LP: '/courts.net.ws'
    };
    StateMachine.prototype.directing = function() {};
    StateMachine.prototype.reading = function() {};
    StateMachine.prototype.changestate = function(newstate, code) {
      return newstate;
    };
    StateMachine.prototype.request = function(code) {
      var docType, route;
      if (currentstate === directing) {
        docType = code.substring(1, 2);
        route = docRoutes[docType];
        if (route != null) {
          return changestate('directing', replace('%', code));
        } else {
          return alert("Barcode not recognized");
        }
      }
    };
    return StateMachine;
  })();
}).call(this);
