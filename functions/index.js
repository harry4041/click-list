// TODO
// Save the value of the weekly/daily etc result

const functions = require("firebase-functions");

// The Firebase Admin SDK to access Firestore.
const admin = require("firebase-admin");
admin.initializeApp(functions.config().firebase);
const dbCon = admin.database().ref("/users/");

// Runs weekly on Mondays at 00:01
exports.dailyReset = functions.pubsub.schedule("01 0 * * *")
    .timeZone("Europe/London")
    .onRun((context) => {
      dbCon.once("value", function(snapshot) {
        snapshot.forEach(function(child) {
          const oldClickValue = child.val().click;
          const oldPB = child.val().lastDailyPB;
          const daily = child.val().lastDaily;
          let updatePB = null;
          if (oldPB < oldClickValue) {
            updatePB = oldClickValue;
          } else {
            updatePB = oldPB;
          }
          console.log(oldClickValue);
          child.ref.update({
            lastDaily: daily,
            lastDailyPB: updatePB,
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
          const oldPB = child.val().lastWeeklyPB;
          const weekly = child.val().lastWeekly;
          let updatePB = null;
          if (oldPB < oldClickValue) {
            updatePB = oldClickValue;
          } else {
            updatePB = oldPB;
          }
          console.log(oldClickValue);
          child.ref.update({
            lastWeekly: weekly,
            lastWeeklyPB: updatePB,
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
          const oldPB = child.val().lastMonthlyPB;
          const monthly = child.val().lastWeekly;
          let updatePB = null;
          if (oldPB < oldClickValue) {
            updatePB = oldClickValue;
          } else {
            updatePB = oldPB;
          }
          console.log(oldClickValue);
          child.ref.update({
            lastMonthly: monthly,
            lastMonthlyPB: updatePB,
            clickMonthly: 0,
            monthlyMidnight: "monthlyMidnight",
          });
        });
      });
    });
