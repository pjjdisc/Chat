var user = { head: 'head/cat.jpg', userid: 123456 };

$(function () {

    //$('#loading').show();

    if (typeof (EventSource) == "undefined") {
        //浏览器不支持EventSource，提示更换浏览器
        var w = document.body.clientWidth;
        var h = document.body.clientHeight;
        $('#messageDiv').css({ 'left': (w / 2 - 450) + 'px', 'top': (h / 2 - 300) + 'px' });
        $('#messageDiv').show();
    }

    var eventObj = new EventSource("ImServer.ashx");
    //获取用户列表
    eventObj.onmessage = function(event) {
        var temp = JSON.parse(event.data);
        if (temp.action == 'pushMsg') {
            addOthersMessage(temp.msg);
        }
    };

    $('#btnSend').click(function() {
        var text = $('#txtContent').val().trim();
        text = replace(text);
        if (text == '') {
            return;
        } else {
            addMeMessage(text);
        }

        $('#txtContent').val('');
    });

    $('#txtContent').keydown(function (e) {
        if (e.keyCode == 13) {
            var text = $('#txtContent').val().trim();
            text = replace(text);
            if (text == '') {
                return false;
            } else {
                addMeMessage(text);
            }

            $('#txtContent').val('');
            return false;
        }
        return true;
    });
});

var replace = function(text) {
    var arr = text.split('\n');
    return arr.join('<br/>');
};

var addOthersMessage = function (obj) {
    var li = $('<li></li>');
    li.addClass('others');
    var img = $('<img class="img-circle smimg" />');
    img.attr('src', obj.head);
    li.append(img);

    var span = $('<span class="chat_others_content"></span>');
    span.html(obj.text);
    li.append(span);

    $('#messageList').append(li);
};

var addMeMessage = function(text) {
    var li = $('<li></li>');
    li.addClass('me info');

    var span = $('<span class="chat_others_content"></span>');
    span.html(text);
    li.append(span);

    var img = $('<img class="img-circle smimg" />');
    img.attr('src', user.head);
    li.append(img);

    $('#messageList').append(li);


};

var addUser = function(item) {
    var ul = $('#userList');
    var li = $('<li></li>');
    li.html('<img src="' + item.Head + '" class="img-circle" /><span>' + item.Name + '</span>');
    ul.append(li);
};

var sendAjax = function(obj, callback) {
    var fun = callback;
    var url = "ImServer.ashx";
    $.ajax({
        type: "POST",
        url: url,
        data: obj,
        success: function (obj) {
            if (typeof (fun) == "function") {
                fun(obj);
            }
        }
    });
};

//将自己加入聊天列表中
sendAjax({
    'action': 'addUser',
    'uid': getParam('uid')
});


var getParam = function(key) {
    var url = Window.location.href;
    var arr = url.split('?');

    if (arr.length == 1) {
        return '';
    }

    var temp = arr[1];

    arr = temp.split('&');

    for (var i = 0, len = arr.length; i < len; i++) {
        var item = arr[i];
        if (item.indexOf(item) > -1) {
            return item.split('=')[1];
        }
    }
    return '';
};