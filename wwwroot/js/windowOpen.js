
//固定視窗大小，無捲軸
function openwindow(url, name, iWidth, iHeight) {
    var url;     //網頁位置;
    var name;    //網頁名稱;
    var iWidth;  //視窗的寬度;
    var iHeight; //視窗的高度;
    var iTop = (window.screen.availHeight - 30 - iHeight) / 2;  //視窗的垂直位置;
    var iLeft = (window.screen.availWidth - 10 - iWidth) / 2;   //視窗的水平位置;
    var newwin = window.open(url, name, 'height=' + iHeight + ',,innerHeight=' + iHeight + ',width=' + iWidth + ',innerWidth=' + iWidth + ',top=' + iTop + ',left=' + iLeft + ',status=no,location=no,status=no,menubar=no,toolbar=no,resizable=no,scrollbars=no');
    newwin.focus();
}

//無固定視窗大小，有捲軸
function openwindowS(url, name, iWidth, iHeight) {
    var url;     //網頁位置;
    var name;    //網頁名稱;
    var iWidth;  //視窗的寬度;
    var iHeight; //視窗的高度;
    var iTop = (window.screen.availHeight - 30 - iHeight) / 2;  //視窗的垂直位置;
    var iLeft = (window.screen.availWidth - 10 - iWidth) / 2;   //視窗的水平位置;
    var newwin = window.open(url, name, 'height=' + iHeight + ',,innerHeight=' + iHeight + ',width=' + iWidth + ',innerWidth=' + iWidth + ',top=' + iTop + ',left=' + iLeft + ',status=no,location=no,status=no,menubar=no,toolbar=no,resizable=yes,scrollbars=yes');
    newwin.focus();
}