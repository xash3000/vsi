class Equality:

    def __eq__(self, other):
        return self.__dict__ == other.__dict__
