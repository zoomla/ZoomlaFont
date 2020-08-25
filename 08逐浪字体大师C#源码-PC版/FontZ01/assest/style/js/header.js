var headerTemplate = `
<nav class="navbar navbar-expand-md inav"><router-link class="navbar-brand" to="/"><img src="style/images/icon2.jpg" alt="智图" />
<p><strong><a href="#" onclick="call.sys.openurl('https://p.ziti163.com/Class_1/NodePage?author=%E8%80%81Tan%E9%85%B8%E8%8F%9C')">欢迎[老Tan酸菜]</a></strong>
<span id="time"></span></p>
</router-link>
  <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarsExample04" aria-controls="navbarsExample04" aria-expanded="false" aria-label="Toggle navigation"> <span class="navbar-toggler-icon"></span> </button>
  <div class="collapse navbar-collapse" id="navbarsExample04">
    <ul class="navbar-nav ml-auto mr-auto">
		<li :class="'nav-item ' + nav1"><router-link class="nav-link" to="/">精选图库 <span class="sr-only">(current)</span></router-link></li>
		<li :class="'nav-item ' + nav2"><router-link class="nav-link" to="/photo">我的图库</router-link></li>
		<li :class="'nav-item ' + nav3"><router-link class="nav-link" to="/ai">AI识图</router-link></li>
		<li :class="'nav-item ' + nav4"><router-link class="nav-link" to="/font">字库中心</router-link></li>
		<li :class="'nav-item ' + nav5"><router-link class="nav-link" to="/apps">应用中心</router-link></li>
		<li :class="'nav-item ' + nav6"><router-link class="nav-link" to="/setting">设置</router-link></li>
		<li class="nav-item dropdown"> <a class="nav-link dropdown-toggle" href="#" id="dropdown04" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">生态</a>
        <div class="dropdown-menu" aria-labelledby="dropdown04">
		<a class="dropdown-item" href="http://tool.73ic.com" target="_blank">站长工具</a>
		<a class="dropdown-item" href="http://www.ziti163.com/webfont" target="_blank">webfont</a>
		<a class="dropdown-item" href="http://www.ziti163.com/uni" target="_blank">Unicode</a>
		<a class="dropdown-item" href="http://code.z01.com/v4" target="_blank">Bootstrap中国站</a></div>
      </li>
    </ul>
    <form class="form-inline">
      <a href="#" onclick="call.sys.openurl('http://p.ziti163.com')"><i class="zi zi_home"></i></a>
      <a href=""><i class="zi zi_search"></i></a>
      <a href=""><i class="zi zi_fly"></i></a>
      <a href=""><i class="zi zi_user"></i></a>
      <a href=""><i class="zi zi_menu"></i></a>
    </form>
  </div>
</nav>
    `
Vue.component('my-header', {
    template: headerTemplate,
	  props: {
	nav1:String,
	nav2:String,
	nav3:String,
	nav4:String,
	nav5:String,
	nav6:String,
  },
    data() {
    	return {
    		aa:true
    	}
          
      },
    	methods: {
//    		closefun(){
//    			console.log(this.$parent)
//    			this.$emit('hello',"child2parent?")
//    		}
    	}
    // ...
})
