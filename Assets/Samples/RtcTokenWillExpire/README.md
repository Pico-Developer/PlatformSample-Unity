测试token过期行为

# case1：token过期通知
在token过期的前30s时，会受到OnTokenWillExpire通知。

# case2：UpdateToken的作用
如果勾选自动更新token，则调用UpdateToken接口。  
只要保持自动更新token，就不会被踢出房间。一旦取消自动更新token，到期之后就会被踢出房间。  

# case3：重新进房
被踢出房间后，点击JoinRoom按钮，可以重新进房。  


