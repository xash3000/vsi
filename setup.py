from setuptools import setup, find_packages
from vsi import __version__

setup(name="vsi",
      version=__version__,
      url="https://github.com/afaki077/vsi",
      description="very simple interpreter written in pure python",
      author="Ali Faki",
      author_email="alifaki077@gmail.com",
      license="MIT",
      packages=find_packages(),
      scripts=["bin/vsi"]
      )
