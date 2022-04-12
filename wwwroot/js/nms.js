function s(DirName) {
    setSize("13"); changeImg("small", DirName);
}
function m(DirName) {
    setSize("16"); changeImg("middle", DirName);
}
function b(DirName)
{ setSize("20"); changeImg("big", DirName); }
function l(DirName)
{ setSize("24"); changeImg("large", DirName); }
function changeImg(px,DirName)
{
    setDefault(DirName);
  if(px=="small")
  { document.getElementById("small").href = "javascript://"; document.getElementById("smallimg").src = "/"+DirName+"/images/dot_small_b.jpg"; }
  else if(px=="middle")
  { document.getElementById("middle").href = "javascript://"; document.getElementById("middleimg").src = "/" + DirName + "/images/dot_middle_b.jpg"; }
  else if (px == "big")
  { document.getElementById("big").href = "javascript://"; document.getElementById("bigimg").src = "/" + DirName + "/images/dot_big_b.jpg"; }
  else if (px == "large")
  { document.getElementById("large").href = "javascript://"; document.getElementById("largeimg").src = "/" + DirName + "/images/dot_large_b.jpg"; }
}

function setDefault(DirName)
{ document.getElementById("smallimg").src = "/" + DirName + "/images/dot_small.jpg"; document.getElementById("middleimg").src = "/" + DirName + "/images/dot_middle.jpg"; document.getElementById("bigimg").src = "/" + DirName + "/images/dot_big.jpg"; document.getElementById("largeimg").src = "/" + DirName + "/images/dot_large.jpg"; document.getElementById("small").href = "javascript:s('" + DirName + "');"; document.getElementById("middle").href = "javascript:m('" + DirName + "');"; document.getElementById("big").href = "javascript:b('" + DirName + "');"; document.getElementById("large").href = "javascript:l('" + DirName + "');"; }

function setSize(size)
{var tagName_Array=new Array("p","li","td","strong","caption","div","select","input","a","font","span");var tagNameCount_Array=new Array(tagName_Array.length);var contentArea="DetailPage";var tagName_Num=tagName_Array.length;for(var i=0;i<=tagName_Num-1;i++)
{tagNameCount_Array[i]=document.getElementById(contentArea).getElementsByTagName(tagName_Array[i]).length;}
for(var i=0;i<=tagName_Num-1;i++)
{for(var j=0;j<=tagNameCount_Array[i]-1;j++)
{document.getElementById(contentArea).getElementsByTagName(tagName_Array[i])[j].style.fontSize=size+"px";}}}
