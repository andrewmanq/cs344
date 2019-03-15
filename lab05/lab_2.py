
from probability import BayesNet, enumeration_ask, elimination_ask, gibbs_ask

# Utility variables
T, F = True, False

cancerTest = BayesNet([
    ('Cancer', '', 0.01),
    ('Test1', 'Cancer', {T: 0.90, F: 0.2}),
    ('Test2', 'Cancer', {T: 0.90, F: 0.2})
    ])

"""Uses all methods to compute the answer because why not?"""
def compute3methods(X, e, bn):

    # Compute P(Burglary | John and Mary both call).
    print("---")
    print("enumeration_ask: ")
    print(enumeration_ask(X, e, bn).show_approx())
    print("---")
    # elimination_ask() is a dynamic programming version of enumeration_ask().
    print("elimination_ask: ")
    print(elimination_ask(X, e, bn).show_approx())
    print("---")
    # gibbs_ask() is an approximation algorithm helps Bayesian Networks scale up.
    print("gibbs_ask: ")
    print(gibbs_ask(X, e, bn).show_approx())
    print("---")
    # See the explanation of the algorithms in AIMA Section 14.4.

"""Exercise 5.2
a. P(Cancer | test1 ^ test2)   *using @ as alpha
= @ <P(test1 | Cancer) * P(test2 | Cancer) * P(Cancer), P(test1 | !Cancer) * P(test2 | !Cancer) * P(!Cancer)>
= @ <       0.9        *        0.9        *   0.01   ,         0.2        *         0.2        *     0.99  >
= @ < .0081 , 0.0396 >
= T: .17 F: .83
This makes sense. Cancer has a likelihood of 1%. Test results have a failure rate of 20%. While it's
rare to have both tests with false positives, it's still way rarer to have cancer in general.
"""
print("-------------------------------------likelihood of having cancer if both tests are positive:")
compute3methods('Cancer', dict(Test1=T, Test2=T), cancerTest)

"""
a. P(Cancer | test1 ^ !test2)   *using @ as alpha
= @ <P(test1 | Cancer) * P(!test2 | Cancer) * P(Cancer), P(test1 | !Cancer) * P(!test2 | !Cancer) * P(!Cancer)>
= @ <       0.9        *        0.1        *   0.01   ,         0.2        *         0.8        *       0.99  >
= @ < .0009 , 0.1584 >
= T: .00565 F: .994
As with the previous case, the likelihood of a false positive on test 1 is high, especially when test 2
is revealed to be negative.
"""
print("-------------------------------------likelihood of having cancer if only one test is positive:")
compute3methods('Cancer', dict(Test1=T, Test2=F), cancerTest)
