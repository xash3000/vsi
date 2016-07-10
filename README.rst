vsi
---

very simple interpreter written in pure python without any
third party dependencies.


Installation
------------

you will need `python <https://www.python.org/downloads/>`_ interpreter and
`pip <https://pip.pypa.io/en/stable/installing/>`_ package manager
after that run this command

.. code-block:: bash

    $ pip install vsi

**note**: for linux and mac users you may want to prefix the command with sudo


Quick Start
-----------

.. code-block:: ruby

    # this is a comment

    # variables
    x := 1;
    y := x + 2;

    # printing
    print x;
    print y;

    # if statements
    if y > x then
        z := y;
    # optional else
    else
        z := x;
    done

    # while statemnt
    while x < 5 do
        x := x + 1;
        print x;
    done


save this to hello.vsi file and then

.. code-block:: bash

    $ vsi hello.vsi


output

.. code-block:: bash

    1
    3
    2
    3
    4
    5

see more examples at examples/ folder


Is it tested
------------

yes, follow the following steps:

.. code-block:: bash

    $ # first clone the repository
    $ git clone https://github.com/afaki077/vsi.git
    $ # change directory to the clone repository
    $ cd vsi
    $ # install required packages for testing
    $ pip install -r test-requirements.txt
    $ # let's run tests
    $ py.test


LICENSE
-------
**MIT**

see LICENSE file for more information.
