const fs = require("fs");
const path = require("path");
const roots = ["src"];
const domains = new Set([
  "auth","order","orders","pos","inventory","product","products","customer",
  "customers","analytics","tasks","task","system","admin","hrm","lrm","crm",
  "shifts","shift","users","user","payment","payments","report","reports"
]);
const regex = /(\'|\")([a-z]+(?:\.[a-z_]+)+)\1/g;
const perms = new Set();
function walk(dir){
  for (const entry of fs.readdirSync(dir, { withFileTypes: true })) {
    const full = path.join(dir, entry.name);
    if (entry.isDirectory()) {
      walk(full);
    } else if (/\.(ts|tsx|md|json)$/i.test(entry.name)) {
      const text = fs.readFileSync(full, "utf8");
      let m;
      while ((m = regex.exec(text)) !== null) {
        const token = m[2];
        const root = token.split(".")[0];
        if (domains.has(root)) {
          perms.add(token);
        }
      }
    }
  }
}
for (const root of roots) {
  walk(root);
}
console.log(Array.from(perms).sort().join("\n"));
