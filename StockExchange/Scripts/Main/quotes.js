$(function () {
    var initialized = false;
    var travelPath = "api/SecureStockExchange/";
    var tdQuotes = [];
    var travelString = "codes=";

    function Init() {
        var data = $('.quoteCls');

        for(var dt in data)
        {
            var _data = $(data[dt]).data("quote");

            if(_data != null && _data != undefined && _data != "")
                tdQuotes.push(_data);
        }
        initialized = true;

        var quotesDtLngth = tdQuotes.length;


        for(var i = 0; i < quotesDtLngth; i++)
        {
            travelString += tdQuotes[i];

            if(i < (quotesDtLngth - 1) )
                travelString += ","
        }
    }
    Init();

    function fetchQuotePrices(arrQuotes)
    {

        $.getJSON(travelPath, $.param( { codes: arrQuotes}), function (data, textStatus, jqXHR) {

            var test = data;
            if (data) {
                var dataLength = data.length;
                var eles = $('td.quoteCls'); 
                for(var i = 0; i < dataLength; i++)
                {
                    var qte = arrQuotes[i];
                    var val = data[i];
                    var ele = $(eles).filter("[data-quote='" + qte + "']");

                    $(ele).text(val);
                }
            }
        });
    }
    fetchQuotePrices(tdQuotes);

    // timed evt     travelString
    if(tdQuotes.length > 0)
    setInterval(function () {
        fetchQuotePrices(tdQuotes);
    }, 10000);



});