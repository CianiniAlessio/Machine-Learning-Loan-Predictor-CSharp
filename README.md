# Machine-Learning-Loan-Predictor-CSharp
Nowdays getting a loan is getting more and more difficult and the answer to know if a person is eligible fora
loan or not may take some time to be gathered.
Therefore In this project my main goal is to predict if the amount of loan that a person asked based on
different variable is going to be given.

To solve this ML problem I'm going to use the Classification technique, that is used to estimate a discrete label function. Of course I'm trying to find an approximation $h$.
The variables called **features** are: 
**ApplicantIncome,CoapplicantIncome,LoanAmount,Credit_History**  and the final predicted result is called label or class.
The first one normally are numerical and the second one categorical. Since I'm trying to find a function that describes my data, having categorycal labels would be a problem, therefore I'm going to convert them in numerical labels.

I downloaded from Kaggle a dataset in which there are different features for each person, I used just some of these features, and of course the label in the last column because Logistic regression can be prone to overfitting when there is a high number of predictor variables in the model.

Every person has in the last column the 'loan Status' which can be either Y or N, labeled as "1" and "0" from me; converting this categorical values in a possibility of having either 1 or 0 (binary problem).

### LOGISTIC REGRESSION
This type of statistical model, known as logit model, is often used for classification and predictive analytics. Logistic regression estimates the probability of an event occurring.

Since the outcome is a probability, the dependent variable is bounded between 0 and 1. In logistic regression, a logit transformation is applied on the odds;that is, the probability of success divided by the probability of failure. This is also commonly known as the log odds, or the natural logarithm of odds, and this logistic function is represented by the following formulas:
$Logit(y) = \frac{1}{(1+e^{-y})}$ and $ln \left( \frac{y}{1-y} \right )$   = $\beta_0 + \beta_1x_1 +...+\beta_k*x_k$

The beta parameter, or coefficient, in this model is commonly estimated via maximum likelihood estimation (MLE). This method tests different values of beta through multiple iterations to optimize for the best fit of log odds. All of these iterations produce the log likelihood function, and logistic regression seeks to maximize this function to find the best parameter estimate.


Once the optimal coefficient (or coefficients if there is more than one independent variable) is found, the conditional probabilities for each observation can be calculated, logged, and summed together to yield a predicted probability. For binary classification, a probability less than .5 will predict 0 while a probability greater than 0.5 will predict 1.

### ACCORD LIBRARY

In *Accord* there is a Class called IterativeReweightedLeastSquares class and using its constructor you have to decide if you want to use the Linear Regression or Logistic Regression, that's why I passed the logistic Regression. Also you can decide how many iterations your model has to do before completing the training.

Through the constructor you can see some properties such as: coefficients, weights, standard Error.

In the method you have to use first the Learn method, in which you have to pass of course the trainingInputs, then the Decide method in which you pass the testInputs.
I also calculated the relation between the given input vector and its most strongly associated class using the Score(testInputs) method.

After I tested the model and I save the outputs in a vector it will have 2 results: true and falsewhich will be converted in 1 and 0 respectively.
Then I compare the prediction with the testOutputs to see the accuracy of my model.

Since it's a problem in which you have a Binary outcome(label) and different features I decided to create a **Logistic Regression Model** or **Logit model** to train.
```
var learner = new IterativeReweightedLeastSquares<LogisticRegression>()
            {
                MaxIterations = 100
            };
```
The regression coefficient are estimated using the maximum likelihood estimation. Is not possible to find the coefficient without iterating through our model, therefore some process is needed.
Defining the Bernoulli probability density function $pdf = q =\sigma(\theta^T\hat{y}(n))$ and our sample of 'people' as $y(n) = y(1)...y(N)$
The log-likelihood function is the following:
$$l(\theta;x,y) = \sum_{n=1}^N{x(n)\theta^Ty(n) - log(1+e^{\theta^Ty(n)})}$$ which is, unfortunately, a non-linear function of the parameter $\theta$ which can only be maximized by numerical methods. To maximize the log-likelihood we are going to impose that it's gradient is equals to 0, i.e:
$$\nabla_\theta l(\theta;x,y)=\sum_{n=1}^Ny(n){x(n)-q(\theta;y(n))} = 0$$ which is a not solvable system since the number of unknown parameters are superior to the eqations.
The iterative Reweighted Least Squares is used for example for binary problem like the one that we are going to analyze.
This method is equivalent to find the maximum log-likelihood function for a Bernoulli (0,1) distributed process using Newton-Rapson method.
N-R method:
$$x_1 = x_0 -\frac{f_{x_0}}{f^1_{x_0}}$$
Defining the Hessian matrix H after some calculation which are easy to find on the internet we obtain that $$H(\theta,\hat{y}) = Y^TM(\theta,\hat{y})Y$$ where $$M(\theta,\hat{y}) := diag\text{  }{\sigma(\theta^T\hat{y}(1))(1-\sigma(\theta^T\hat{y}(1))}),...,\sigma(\theta^T\hat{y}(N))(1-\sigma(\theta^T\hat{y}(N))$$.
Then the N-R algorithm can be written as : $$\theta_{m+1} = \theta_m-H^{-1}(\theta_m,\hat(y))\nabla_\theta l(\theta_m;\hat{x},\hat{y})$$
After some step the goal is to minimize the M-norm, this means: $$\min_{\theta_{m+1}}=||Y\theta_{m+1} - z_m||^2_{M(\theta_m,\hat{y})}$$ with $$z_m = Y\theta_m -M(\theta_m,\hat{y})^{-1}(x-\hat{p}(\theta_m,y)$$


After this I pass to the logit model the inputs and the output created before in order to creates some weight to predict a correct label for each testInput, I convert the solution from bool to binary, and then I print the weights.
```
                // Train a Logistic Regression model
                
                LogisticRegression logit = learner.Learn(traingInputs, traingOutputs);

                // Predict output 
                predictions = logit.Decide(testInputs);

                // Calculate the relation between the given input vector and its most strongly associated class
                scores = logit.Score(testInputs);
                

                Console.Write("WEIGHTS: ");
                foreach (var k in logit.Weights)
                {
                    Console.Write($"{Math.Round(k, 5)}, ");
                }
```
With this part of code I output the results:
```
            Console.WriteLine($"\nThe regression used is : {logit.Linear}");
            Console.WriteLine($"\nThe standard error encountered is : {logit.StandardErrors.Mean()}");

            foreach (var k in testOutputs)
            {
                if (k == predictions[i].ToZeroOne())
                {
                    correctNumber++;
                }
                i++;
            }

            Console.WriteLine($"\nThe accuracy of my model is = {correctNumber * 100 / testOutputs.Length}%");
```
