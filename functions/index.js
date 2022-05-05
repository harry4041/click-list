const functions = require("firebase-functions");

// The Firebase Admin SDK to access Firestore.
const admin = require("firebase-admin");
admin.initializeApp(functions.config().firebase);

exports.scheduledFunctionCrontab = functions.pubsub.schedule("* * * * *")
    .timeZone("America/New_York")
    .onRun((context) => {
      const database = admin.database();
      const ref = database.ref().child("users")
          .child("EieBJdkvdGO9NZCjKAkkRNxraRo1").child("click");
      ref.set(0);
    });
