$(function() {
    /*$.ajax({
      crossOrigin: true,
      url: "https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=en-US",
      success: function(data) {
		var bingObject = JSON.parse(data);
        console.log(bingObject);
		var imageUrl = "https://www.bing.com"+bingObject.images[0].url;
		document.getElementById("hello").style.backgroundImage = "url('"+imageUrl+"')";
      }   
	});*/ 
	getFile("https://www.bing.com/HPImageArchive.aspx?format=xml&idx=0&n=7&mkt=en-US", "xml",
		function(response)
		{
			var xmlDoc = jQuery.parseXML(response.responseText);
			console.log(xmlDoc);
			
			var images = [];
			for(var i=0; i<7; i++){
				images[i] = "https://www.bing.com"+xmlDoc.getElementsByTagName("urlBase")[i].textContent+"_1920x1080.jpg";
				document.getElementById("div_img_"+i).style.backgroundImage = "url('"+images[i]+"')";
				document.getElementById("a_img_"+i).setAttribute('href', images[i]);
				document.getElementById("text_img_"+i).innerHTML = xmlDoc.getElementsByTagName("copyright")[i].textContent.split("(")[0];
			}

			document.getElementById("hello").style.backgroundImage = "url('"+images[0]+"')";
		}
	)
}); 


// callback is optional, since jQuery has promises
function getFile(theURL, type, callback)
{
	/*
	* Original jQuery.ajax mid - CROSS DOMAIN AJAX 
	* @author James Padolsey (http://james.padolsey.com)
	* @updated 12-JAN-10
	* @info http://james.padolsey.com/javascript/cross-domain-requests-with-jquery/
	* source: https://raw.github.com/padolsey/jquery.fn/master/cross-domain-ajax/jquery.xdomainajax.js
	*
	* This version adds a fix for correctly handling format:xml
	*/
	jQuery.ajax = (function(_ajax)
	{
		var protocol = location.protocol,
			hostname = location.hostname,
			exRegex = RegExp(protocol + '//' + hostname),
			YQL = 'http' + (/^https/.test(protocol)?'s':'') + '://query.yahooapis.com/v1/public/yql?callback=?',
			query = 'select * from html where url="{URL}" and xpath="*"';

		function isExternal(url)
		{
			return !exRegex.test(url) && /:\/\//.test(url);
		}

		return function(o)
		{
			var url = o.url;
			if (o.dataType == 'xml')   // @rickdog - fix for XML
				query = 'select * from xml where url="{URL}"';	// XML
			if ( /get/i.test(o.type) && !/json/i.test(o.dataType) && isExternal(url) )
			{
				// Manipulate options so that JSONP-x request is made to YQL
				o.url = YQL;
				o.dataType = 'json';
				o.data = {
					q: query.replace('{URL}', url + (o.data ? (/\?/.test(url) ? '&' : '?') + jQuery.param(o.data) : '')),
					format: 'xml'
				};

				// Since it's a JSONP request
				// complete === success
				if (!o.success && o.complete) {
					o.success = o.complete;
					delete o.complete;
				}

				o.success = (function(_success)
				{
					return function(data)
					{
						if (_success) {
							// Fake XHR callback.
							_success.call(this, {
								// YQL screws with <script>s, Get rid of them
								responseText: (data.results[0] || '').replace(/<script[^>]+?\/>|<script(.|\s)*?\/script>/gi, '')
							}, 'success');
						}
					};
				})(o.success);
			}
			return _ajax.apply(this, arguments); // not special, use base Jquery ajax
		};
	})(jQuery.ajax);


	return $.ajax({
		url: theURL,
		type: 'GET',
		dataType: type,
		success: function(res) {
			// var text = res.responseText;
			// .. then you can manipulate your text as you wish
			callback ? callback(res) : undefined;
		}
	})
};