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
var learner = new IterativeReweightedLeastSquares<LogisticRegression>()
            {
                MaxIterations = 100
            };
```
The regression coefficient are estimated using the maximum likelihood estimation. Is not possible to find the coefficient without iterating through our model, therefore some process is needed.
Defining the Bernoulli probability density function $p =\sigma(\theta^T\hat{y}(t))$
The log-likelihood function is the following:
$$l(\theta;x,y) = \sum_{t=1}^N{x(t)\theta^Ty(t) - log(1+e^{\theta^Ty(t)})}$$ which is, unfortunately, a non-linear function of the parameter $\theta$ which can only be maximized by numerical methods. To maximize the log-likelihood we are going to the it's gradient equals to 0, i.e:
$$\nabla_\theta l(\theta;x,y)=\sum_{t=1}^Ny(t){x(t)-p_1(\theta;y(t))} = 0$$ which is a not solvable system since the number of unknown parameters are superior to the eqations.
The iterative Reweighted Least Squares is used for example for binary problem like the one that we are going to analyze.
This method is equivalent to find the maximum log-likelihood function for a Bernoulli (0,1) distributed process using Newton-Rapson method.
N-R method:
$$x_1 = x_0 -\frac{f_{x_0}}{f^1_{x_0}}$$
Defining the Hessian matrix H after some calculation which are easy to find on the internet we obtain that $$H(\theta,\hat{y}) = Y^TM(\theta,\hat{y}Y$$ where $$M(\theta,\hat{y}) := diag{\sigma(\theta^T\hat{y(1))(1-\sigma(\theta^T\hat{y}(1))})}),...,\sigma(\theta^T\hat{y}(N))(1-\sigma(\theta^T\hat{y}(N))$$.
Then after m-steph the N-R algorithm can be written as : $$\theta_{m+1} = \theta_m-H^{-1}(\theta_m,\hat(y)\nabla_\theta l(\theta_m;\hat{x},\hat{y})$$

