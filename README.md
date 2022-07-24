# Machine-Learning-Loan-Predictor-CSharp
Nowdays getting a loan is getting more and more difficult and the answer to know if a person is eligible fora
loan or not may take some time to be gathered.
Therefore In this project my main goal is to predict if the amount of loan that a person asked based on
different variable is going to be given.
These variables are: 
**ApplicantIncome,CoapplicantIncome,LoanAmount,Credit_History** .

I downloaded from Kaggle a dataset in which there are different features for each person, I used just some of these features.

Every person has in the last column the 'loan Status' which can be either Y or N, labeled as "1" and "0" from me.

Since it's a problem in which you have a Binary outcome(label) and different features I decided to create a **Logistic Regression Model** or **Logit model** to train.
```
function test() {
  console.log("notice the blank line before this function?");
}
```

''' 
var learner = new IterativeReweightedLeastSquares<LogisticRegression>()
            {
                MaxIterations = 100
            };
'''
When we talk about the **Iterative Reweighted Least Squares** method the aim is to find a *maximum estimator* for the generalized model. A **Logistic Regression Model** in regression analysis is estimating the parameters of a logistic model, therefore its coefficient in the linear combination.

