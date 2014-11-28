//>>built
define("epi/shell/widget/dialog/Dialog",["dojo/_base/array","dojo/_base/declare","dojo/_base/lang","dojo/dom-class","dojo/dom-style","dojo/promise/all","epi","epi/shell/widget/dialog/_DialogBase","epi/shell/widget/_ActionConsumerWidget","epi/shell/widget/Toolbar","epi/shell/widget/ToolbarSet"],function(_1,_2,_3,_4,_5,_6,_7,_8,_9,_a,_b){return _2([_8,_9],{confirmActionText:_7.resources.action.ok,cancelActionText:_7.resources.action.cancel,contentClass:"",dialogClass:"epi-dialog-landscape",defaultActionsVisible:true,setFocusOnConfirmButton:true,destroyOnHide:false,_preferredHeight:0,_validators:null,_okButtonName:"ok",_cancelButtonName:"cancel",postMixInProperties:function(){this.inherited(arguments);this._validators=[];if(this.content&&this.content.onSubmit){this.connect(this.content,"onSubmit",this._onSubmit);}},postCreate:function(){this.inherited(arguments);this.set("definitionConsumer",new _b({baseClass:"dijitDialogPaneActionBar",layoutContainerClass:_a},this.actionContainerNode));},_size:function(){_5.set(this.containerNode,{minHeight:this.get("_preferredHeight")+"px"});this.inherited(arguments);_5.set(this.containerNode,{minHeight:""});_1.forEach(["height","width"],function(_c){if(this[_c]==="auto"){this[_c]="";}},this.containerNode.style);},addValidator:function(_d){this._validators.push(_d);},getActions:function(){var _e=this.inherited(arguments);if(this.defaultActionsVisible){var _f={name:this._okButtonName,label:this.confirmActionText,title:null,settings:{type:"submit"}},_10={name:this._cancelButtonName,label:this.cancelActionText,title:null,action:_3.hitch(this,this._onCancel)};if(this.setFocusOnConfirmButton){_f.settings["class"]="Salt";}else{_10.settings={"class":"Salt",firstFocusable:true};}_e.push(_f,_10);}return _e;},_onCancel:function(){this.onCancel();},_onSubmit:function(){if(!this.validate()){return;}if(this._validators.length===0){this.inherited(arguments);return;}var _11=_1.map(this._validators,function(_12){return _12();});var _13=arguments;var _14=new _6(_11);_14.then(_3.hitch(this,function(_15){if(_1.some(_15,function(_16){return !_16[0];})){return;}this.inherited(_13);}));},_setContentClassAttr:function(_17){_4.remove(this.containerNode,this.contentClass);this._set("contentClass",_17);_4.add(this.containerNode,this.contentClass);},_get_preferredHeightAttr:function(){if(!!this.dialogClass&&this._preferredHeight<25){this._preferredHeight=_5.get(this.containerNode,"height");}return this._preferredHeight;}});});