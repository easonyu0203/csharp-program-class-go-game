const promptt = require("prompt");

promptt.start();

promptt.get(["data"], function (err: any, result: { data: string }) {
  if (err) {
    return onErr(err);
  }
  console.log("send data: " + result.data);
  if (result.data == "x") {
    return;
  }
});

function onErr(err: any) {
  console.log(err);
  return 1;
}
