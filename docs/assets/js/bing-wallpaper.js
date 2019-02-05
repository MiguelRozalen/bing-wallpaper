$(function() {
   $.ajax({
      crossOrigin: true,
      url: "https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=8&mkt=en-US",
      success: function(data) {	  
		var bingObject = JSON.parse(data);
		var images = bingObject.images;
		var imageUrl = "https://www.bing.com"+bingObject.images[0].url;
		document.getElementById("hello").style.backgroundImage = "url('"+imageUrl+"')";
		for(var i=0; i<7; i++){
			document.getElementById("div_img_"+i).style.backgroundImage = "url('https://www.bing.com"+images[i].urlbase+"_1920x1080.jpg')";
			document.getElementById("a_img_"+i).setAttribute('href', "https://www.bing.com"+images[i].urlbase+"_1920x1080.jpg");
			document.getElementById("text_img_"+i).innerHTML = images[i].copyright;
		}
      }   
   });
}); 
