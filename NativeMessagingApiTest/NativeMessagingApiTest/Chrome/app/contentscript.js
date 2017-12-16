var postInfo = $("#blog_post_info_block");
if(postInfo.length!=1){
	chrome.runtime.sendMessage({type:"cnblog-article-information", error:"获取文章信息失败."});
}
else{
	var msg = {
		type: "cnblog-article-information",
		title : $("#cb_post_title_url").text(),
		postDate : postInfo.find("#post-date").text(),
		author : postInfo.find("a").first().text(),
		url: document.URL
	};
	chrome.runtime.sendMessage(msg);
}

chrome.extension.onMessage.addListener(
    function (request, sender, sendResponse) {
        if (request.message == "setTitel") {
            $("#cb_post_title_url").text("aaa");
            sendResponse({ result: "告诉你" })
        }
        else {
            $("#cb_post_title_url").text("bcd");
            sendResponse({ result: "不告诉你" })
        }
            
    });  
