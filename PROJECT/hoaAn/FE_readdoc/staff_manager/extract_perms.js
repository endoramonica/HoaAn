const fs = require("fs");
const text = fs.readFileSync("src/utils/constants.ts", "utf8");
const regex = /["']([a-z]+(?:\.[a-z_]+)+)["']/g;
const vals = new Set();
let m;
while ((m = regex.exec(text)) !== null) {
  vals.add(m[1]);
}
console.log(Array.from(vals).sort().join("\n"));
