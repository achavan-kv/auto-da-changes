(function() {
  require(['jquery', 'chosen.jquery', 'jquery.tmpl', 'tsorter', 'test.data', 'jquery-ui-1.8', 'BarCodeScanner', 'ScanRedirect', 'StateMachine'], function($) {
    var Table1Sorter, dataItem, _fn, _fn2, _fn3, _i, _j, _k, _len, _len2, _len3, _ref, _ref2, _ref3;
    if (($('#pickingtable').length)) {
      Table1Sorter = new TSorter;
      Table1Sorter.init('pickingtable');
    }
    document.addEventListener('keyup', function(e) {
      if (e.altKey) {
        if (e.keyCode === 83) {
          return $('#search').click();
        } else if (e.keyCode === 72) {
          return window.location.href = $('#home').attr('href');
        } else if (e.keyCode === 73) {
          return $('#help').click();
        } else if (e.keyCode === 77) {
          return $('#maximize').click();
        } else if (e.keyCode === 76) {
          return window.location.href = $('#logoff').attr('href');
        }
      }
    }, false);
    if (($('#deliveryItem').length)) {
      _ref = window.data.DeliveryItems;
      _fn = function(dataItem) {
        return $('#deliveryItem').tmpl(dataItem).appendTo($('#pickingtable tbody'));
      };
      for (_i = 0, _len = _ref.length; _i < _len; _i++) {
        dataItem = _ref[_i];
        _fn(dataItem);
      }
      $('#pickingtable tbody select').chosen({
        allow_single_deselect: true
      });
    }
    if (($('#deliveryItemList').length)) {
      _ref2 = window.data.DeliveryItems;
      _fn2 = function(dataItem) {
        return $('#deliveryItemList').tmpl(dataItem).appendTo($('#loadlist'));
      };
      for (_j = 0, _len2 = _ref2.length; _j < _len2; _j++) {
        dataItem = _ref2[_j];
        _fn2(dataItem);
      }
    }
    if (($('#picklistprintscript').length)) {
      _ref3 = window.data.DeliveryItems;
      _fn3 = function(dataItem) {
        return $('#picklistprintscript').tmpl(dataItem).appendTo($('#picklistprint tbody'));
      };
      for (_k = 0, _len3 = _ref3.length; _k < _len3; _k++) {
        dataItem = _ref3[_k];
        _fn3(dataItem);
      }
    }
    $('.clearbtn').click(function() {
      $('.chosenbox').val(0);
      return $('.chosenbox').trigger("liszt:updated");
    });
    $("#loadlist").sortable({
      revert: 200,
      scope: '#loadlist',
      cancel: '.th',
      items: ">div:not(.th)"
    });
    $("#loadlist").disableSelection();
    $('#pickingtable select').change(function() {
      if (($(this).parents('tr').find('input:checked').length)) {
        $('#pickingtable input:checked').parents('tr').find('select').val($(this).val());
        return $('.truckselect').trigger("liszt:updated");
      }
    });
    $('#clearchecks').click(function() {
      $('#pickingtable input:checkbox').prop("checked", false);
      return $('#pickingtable .highlight').removeClass('highlight');
    });
    $('#addchecks').click(function() {
      $('#pickingtable input:checkbox').prop("checked", true);
      return $('#pickingtable tr').addClass('highlight');
    });
    $('#pickingtable input:checkbox').change(function() {
      if ($(this).is(':checked')) {
        return $(this).parents('tr').addClass('highlight');
      } else {
        return $(this).parents('tr').removeClass('highlight');
      }
    });
    $('#pickingtable .unloadtruck').addClass('hidden');
    $('#pickingtable .loadtruck').find('button').click(function() {
      $(this).parents('tr').find('td:gt(2)').addClass('hidden');
      $(this).parents('tr').find('td:first').removeClass('hidden');
      return $(this).parents('tr').appendTo($('#pickedtable tbody'));
    });
    $('#pickingtable .unloadtruck').click(function() {
      $(this).parents('tr').find('.hidden').removeClass('hidden');
      $(this).parents('tr').find('td:first').addClass('hidden');
      return $(this).parents('tr').appendTo($('#pickingtable tbody'));
    });
    $('#removeall').click(function() {
      $('#pickedtable .hidden').removeClass('hidden');
      $('#pickedtable tr').find('td:first').addClass('hidden');
      return $('#pickedtable tbody tr').appendTo($('#pickingtable tbody'));
    });
    $('#addall').click(function() {
      $('#pickingtable tr').find('td:gt(2)').addClass('hidden');
      $('#pickingtable tr').find('td:first').removeClass('hidden');
      return $('#pickingtable tbody tr').appendTo($('#pickedtable tbody'));
    });
    $('#clearassign').click(function() {
      $('#pickingtable select').val(0);
      $('#pickingtable abbr').remove();
      return $('.truckselect').trigger("liszt:updated");
    });
    $('select').chosen({
      allow_single_deselect: true
    });
    return null;
  });
}).call(this);
