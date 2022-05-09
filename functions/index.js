// TODO
// Save the value of the weekly/daily etc result

const functions = require("firebase-functions");

// The Firebase Admin SDK to access Firestore.
const admin = require("firebase-admin");
admin.initializeApp(functions.config().firebase);
const dbCon = admin.database().ref("/users/");

// Runs weekly on Mondays at 00:01
exports.dailyReset = functions.pubsub.schedule("01 0 * * 1")
    .timeZone("Europe/London")
    .onRun((context) => {
      dbCon.once("value", function(snapshot) {
        snapshot.forEach(function(child) {
          const oldClickValue = child.val().click;
          console.log(oldClickValue);
          child.ref.update({
            lastDaily: oldClickValue,
            click: 0,
            dailyMidnight: "dailyMidnight",
          });
        });
      });
    });

// Runs weekly on Mondays at 00:01
exports.weeklyReset = functions.pubsub.schedule("01 0 * * 1")
    .timeZone("Europe/London")
    .onRun((context) => {
      dbCon.once("value", function(snapshot) {
        snapshot.forEach(function(child) {
          const oldClickValue = child.val().clickWeekly;
          console.log(oldClickValue);
          child.ref.update({
            lastWeekly: oldClickValue,
            clickWeekly: 0,
            weeklyMidnight: "weeklyMidnight",
          });
        });
      });
    });

// Runs Monthly on the 1st at 00:01
exports.monthlyReset = functions.pubsub.schedule("01 0 1 * *")
    .timeZone("Europe/London")
    .onRun((context) => {
      dbCon.once("value", function(snapshot) {
        snapshot.forEach(function(child) {
          const oldClickValue = child.val().clickMonthly;
          console.log(oldClickValue);
          child.ref.update({
            lastMonthly: oldClickValue,
            clickMonthly: 0,
            monthlyMidnight: "monthlyMidnight",
          });
        });
      });
    });
