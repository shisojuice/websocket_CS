
/**
 * CONFIRMを付与する関数です。
 * @param {element} HTML_ELEMENT
 * @param {string} msg メッセージ
 * @return {boolean} 真偽
 */
async function showRegister(ele, multiedit) {
    document.documentElement.addEventListener("keydown", keyCancelMsg, true);
    const multi = multiedit ?
        `<div class="myrow">
        <label>　週</label>
         <input type="checkbox" id="mon" name="mon" checked /><label class="week" for="mon">月</label>
         <input type="checkbox" id="tue" name="tue" checked /><label class="week" for="tue">火</label>
         <input type="checkbox" id="wed" name="wed" checked /><label class="week" for="wed">水</label>
         <input type="checkbox" id="thu" name="thu" checked /><label class="week" for="thu">木</label>
         <input type="checkbox" id="fri" name="fri" checked /><label class="week" for="fri">金</label>
         <input type="checkbox" id="sat" name="sat"  /><label class="week" for="sat">土</label>
         <input type="checkbox" id="sun" name="sun"  /><label class="week" for="sun">日</label>
    </div>
    <div class="myrow">
         <label>日付</label><input id="mydatefrom"></input><label>～</label><input id="mydateto"></input>
    </div>`
        :
        `<div class="myrow">
        <label>日付</label><input id="mydate"></input>
    </div>`;

    ele.innerHTML = `
    <div id="myloadback">
    <div id="myload">
        <div class="myrow">
            <label>内容</label><input id="mymsg" autofocus ></input>
        </div>
        ${multi}
        <div class="myrow">
            <label>時刻</label><input id="mytimefrom"></input><label>～</label><input id="mytimeto"></input>
        </div>
        <div class="myrow">
            <label>　色</label><input id="mycolor"></input>
        </div>
        <div class="myrow">
            <button id="mybtnA">OK</button>
            <button id="mybtnC">CANCEL</button>
        </div>
    </div>
    </div>
    <style>
    body { overflow:hidden; } label { min-width: 40px; } .week{ max-width: 20px; min-width: 20px; }
    #myload { min-width: 400px; border-radius: 5px; border:solid 1px gray; background: #FFF; padding:5px; }
    #mymsg { text-align:left; width: 100%; }
    .myrow { display: flex; flex-direction: row; flex-wrap: nowrap; padding-top:5px; }
    #mybtnA { min-height:30px; min-width: 50%; border:none; border-radius: 5px; color: #FFF; background:blue; cursor:pointer;  }
    #mybtnC { min-height:30px; min-width: 50%; border:none;  border-radius: 5px; color: black;  background:gray; cursor:pointer; }
    #myloadback{  background-color: rgba(220, 220, 220, 0.5);position:absolute; top: 0px ; left: 0;display: grid;place-items: center;  text-align: center;background-position: center;z-index:9999;min-height:100vh; min-width:100%;overflow: hidden; } </style>
    `;
    const resultModal = ({ onClose }) => {
        document.getElementById("mybtnA").addEventListener("click", (e) => { onClose(true); });
        document.getElementById("mybtnC").addEventListener("click", (e) => { onClose(null); });
    }

    const ret = await new Promise((resolve) => {
        resultModal({
            onClose: resolve,
        });
    });
    document.documentElement.removeEventListener("keydown", keyCancelMsg, true);
    while (ele.firstChild) {
        ele.removeChild(ele.firstChild);
    }
    return ret;
}
function keyCancelMsg(e) {
    if (e.key !== "Enter" && e.key !== "Tab") {
        e.preventDefault();
        e.stopPropagation();
        return;
    }
    if (e.key == "Enter") {
        e.preventDefault();
        e.stopPropagation();
        if (document.activeElement.id === "mybtnA") {
            document.getElementById("mybtnA").click();
            return;
        }
        if (document.activeElement.id === "mybtnC") {
            document.getElementById("mybtnC").click();
            return;
        }
        document.getElementById("mybtnA").click();
        return;
    }
    if (e.key == "Tab" && e.shiftKey == false) {
        if (document.activeElement.id === "mybtnC") {
            e.preventDefault();
            e.stopPropagation();
            document.getElementById("mymsg").focus();
            return;
        }
        if (document.activeElement === document.querySelector("body")) {
            e.preventDefault();
            e.stopPropagation();
            document.getElementById("mymsg").focus();
            return;
        }
        return;
    }
    if (e.key == "Tab" && e.shiftKey == true) {
        if (document.activeElement.id === "mymsg") {
            e.preventDefault();
            e.stopPropagation();
            document.getElementById("mybtnC").focus();
            return;
        }
        if (document.activeElement === document.querySelector("body")) {
            e.preventDefault();
            e.stopPropagation();
            document.getElementById("mymsg").focus();
            return;
        }
        return;
    }
}