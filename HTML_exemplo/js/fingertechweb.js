/*********************************************
* Nome: Capture
* Descrição: Chama o método "Capture" da aplicação desktop, 
* responsável por chamar a tela de captura de digital para apenas um único dedo.
* Este método é recomendável quando você deseja capturar a impressão digital de um único dedo e 
* não existe a necessidade de identificar qual dedo da mão esta digital pertence. 
* Retorno: Template (String) ou Null
*********************************************/
function Capture() {

	$body = $("body");
	$body.addClass("loading");
	$.ajax({

		url: 'http://localhost:9000/api/getimage/imagem',
		type: 'GET',
		success: function (data) {
			$body.removeClass("loading");
			
			console.log(data.Msg)
			
			if (data != "" && data != null) {
				$("#inputTemplate").attr('src', `data:image/png;base64,${data.Imagem_assinatura}`);
				
				
				alert(data.Msg);
			}
			else {
				alert(data.Msg);
			}
		}
	})
}

$(function() {
	$("#btn-capture").on("click", function(){
		Capture();
	});
	
});