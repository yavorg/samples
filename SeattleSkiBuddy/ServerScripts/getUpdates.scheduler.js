var updatesTable = tables.getTable('updates');
var request = require('request');

function getUpdates() {   
    // Check what is the last tweet we stored when the job last ran
    // and ask Twitter to only give us more recent tweets
    appendLastTweetId(
        'http://search.twitter.com/search.json?q=ski%20weather&result_type=recent&geocode=47.593199,-122.14119,200mi&rpp=100', 
        function twitterUrlReady(url){
            request(url, function tweetsLoaded (error, response, body) {
                if (!error && response.statusCode == 200) {
                    var results = JSON.parse(body).results;
                    if(results){
                        console.log('Fetched new results from Twitter');
                        results.forEach(function visitResult(tweet){
                            if(!filterOutTweet(tweet)){
                                var update = {
                                    twitterId: tweet.id,
                                    text: tweet.text,
                                    author: tweet.from_user,
                                    location: tweet.location || "Unknown location",
                                    date: tweet.created_at
                                };
                                updatesTable.insert(update);
                            }
                        });
                    }            
                } else { 
                    console.error('Could not contact Twitter');
                }
            });
            
        });
}

// Find the largest (most recent) tweet ID we have already stored
// (if we have stored any) and ask Twitter to only return more
// recent ones
function appendLastTweetId(url, callback){
    updatesTable
    .orderByDescending('twitterId')
    .read({success: function readUpdates(updates){
        if(updates.length){
            callback(url + '&since_id=' + updates[0].twitterId + 1);
        } else {
            callback(url);
        }
    }});
}

function filterOutTweet(tweet){
    // Remove retweets and replies
    return (tweet.text.indexOf('RT') === 0 || tweet.to_user_id);
}
