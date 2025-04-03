require(['jquery','chosen.jquery', 'jquery.tmpl', 'tsorter','test.data','jquery-ui-1.8','BarCodeScanner','ScanRedirect','StateMachine'], 
	($) -> 

		if($('#pickingtable').length) 
			Table1Sorter = new TSorter
			Table1Sorter.init('pickingtable')

		document.addEventListener('keyup', (e) ->
			if (e.altKey)
				if (e.keyCode == 83) # 'S'
					$('#search').click()
				else if (e.keyCode == 72) # 'H'
					window.location.href = $('#home').attr('href')
				else if (e.keyCode == 73) # 'I'
					$('#help').click()
				else if (e.keyCode == 77) # 'M'
					$('#maximize').click()
				else if (e.keyCode == 76) # 'L'
					window.location.href = $('#logoff').attr('href')
		, false)

	

		if($('#deliveryItem').length) 
			for dataItem in window.data.DeliveryItems
				do (dataItem) ->
					$('#deliveryItem').tmpl(dataItem).appendTo($('#pickingtable tbody'))
			$('#pickingtable tbody select').chosen({allow_single_deselect:true})

		if($('#deliveryItemList').length) 
			for dataItem in window.data.DeliveryItems
				do (dataItem) ->
					$('#deliveryItemList').tmpl(dataItem).appendTo($('#loadlist'))

		if($('#picklistprintscript').length) 
			for dataItem in window.data.DeliveryItems
				do (dataItem) ->
					$('#picklistprintscript').tmpl(dataItem).appendTo($('#picklistprint tbody'))

		$('.clearbtn').click ->
			$('.chosenbox').val(0)
			$('.chosenbox').trigger("liszt:updated");

		$("#loadlist").sortable({
			revert: 200,
			scope: '#loadlist',
			cancel: '.th',
			items: ">div:not(.th)",
		})

		$("#loadlist").disableSelection()
		
		$('#pickingtable select').change ->
			if ($(this).parents('tr').find('input:checked').length)
				$('#pickingtable input:checked').parents('tr').find('select').val($(this).val())
				$('.truckselect').trigger("liszt:updated")

		$('#clearchecks').click ->
			$('#pickingtable input:checkbox').prop("checked", false)
			$('#pickingtable .highlight').removeClass('highlight')

		$('#addchecks').click ->
			$('#pickingtable input:checkbox').prop("checked", true)
			$('#pickingtable tr').addClass('highlight')
		
		$('#pickingtable input:checkbox').change ->
			if $(this).is(':checked')
				$(this).parents('tr').addClass('highlight')
			else
				$(this).parents('tr').removeClass('highlight')

		$('#pickingtable .unloadtruck').addClass('hidden')

		#--- Picking screen
		$('#pickingtable .loadtruck').find('button').click ->
			$(this).parents('tr').find('td:gt(2)').addClass('hidden')
			$(this).parents('tr').find('td:first').removeClass('hidden')
			$(this).parents('tr').appendTo($('#pickedtable tbody'))

		$('#pickingtable .unloadtruck').click ->
			$(this).parents('tr').find('.hidden').removeClass('hidden')
			$(this).parents('tr').find('td:first').addClass('hidden')
			$(this).parents('tr').appendTo($('#pickingtable tbody'))

		$('#removeall').click ->
			$('#pickedtable .hidden').removeClass('hidden')
			$('#pickedtable tr').find('td:first').addClass('hidden')
			$('#pickedtable tbody tr').appendTo($('#pickingtable tbody'))

		$('#addall').click ->
			$('#pickingtable tr').find('td:gt(2)').addClass('hidden')
			$('#pickingtable tr').find('td:first').removeClass('hidden')
			$('#pickingtable tbody tr').appendTo($('#pickedtable tbody'))

		$('#clearassign').click -> 
				$('#pickingtable select').val(0)
				$('#pickingtable abbr').remove()
				$('.truckselect').trigger("liszt:updated")

		#----- Picking screen end 
		$('select').chosen({allow_single_deselect: true})
		null
)

